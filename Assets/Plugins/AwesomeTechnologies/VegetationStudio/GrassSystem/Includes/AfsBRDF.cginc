#ifndef AFS_BRDF_INCLUDED
#define AFS_BRDF_INCLUDED

// Based on The StandardBRDF from Unity 5.5.

// Define helper functions for unity 5.4.x
// Due to a bug in shader lab in Unity 5.5.0 (fixed in p2) we can not use the UNITY_VERSION macro and have to declare "custom functions"
// #if UNITY_VERSION < 550

	half SmoothnessToPerceptualRoughness_AFS(half smoothness)
	{
		return (1 - smoothness);
	}

	half PerceptualRoughnessToRoughness_AFS(half perceptualRoughness)
	{
		return perceptualRoughness * perceptualRoughness;
	}

	half RoughnessToPerceptualRoughness_AFS(half roughness)
	{
		return sqrt(roughness);
	}

	inline half PerceptualRoughnessToSpecPower_AFS(half perceptualRoughness)
	{
		half m = PerceptualRoughnessToRoughness_AFS(perceptualRoughness);	// m is the true academic roughness.
		half sq = max(1e-4f, m*m);
		half n = (2.0 / sq) - 2.0;							// https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
		n = max(n, 1e-4f);									// prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero
		return n;
	}

	// Note: Disney diffuse must be multiply by diffuseAlbedo / PI. This is done outside of this function.
	half DisneyDiffuse_AFS(half NdotV, half NdotL, half LdotH, half perceptualRoughness)
	{
		half fd90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
		// Two schlick fresnel term
		half lightScatter	= (1 + (fd90 - 1) * Pow5(1 - NdotL));
		half viewScatter	= (1 + (fd90 - 1) * Pow5(1 - NdotV));

		return lightScatter * viewScatter;
	}

// #endif

// New SmithJointGGXVisibilityTerm from Unity 5.5.x
inline half SmithJointGGXVisibilityTerm_AFS (half NdotL, half NdotV, half roughness)
{
#if 0
	// Reorder code to be more optimal
	half a			= roughness;
	half a2			= a * a;

	half lambdaV	= NdotL * sqrt((-NdotV * a2 + NdotV) * NdotV + a2);
	half lambdaL	= NdotV * sqrt((-NdotL * a2 + NdotL) * NdotL + a2);

	// Simplify visibility term: (2.0f * NdotL * NdotV) /  ((4.0f * NdotL * NdotV) * (lambda_v + lambda_l + 1e-5f));
	return 0.5f / (lambdaV + lambdaL + 1e-5f);	// This function is not intended to be running on Mobile,
												// therefore epsilon is smaller than can be represented by half
#else
    // Approximation of the above formulation (simplify the sqrt, not mathematically correct but close enough)
	half a = roughness;
	half lambdaV = NdotL * (NdotV * (1 - a) + a);
	half lambdaL = NdotV * (NdotL * (1 - a) + a);

	return 0.5f / (lambdaV + lambdaL + 1e-5f);
#endif
}

// New GGXTerm from Unity 5.5.x
inline half GGXTerm_AFS (half NdotH, half roughness)
{
	half a2 = roughness * roughness;
	half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
	return 0.31830988618f * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
											// therefore epsilon is smaller than what can be represented by half
}


// //////////////////////////////////////
// The BRDF

half4 BRDF1_AFS_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness, half NdotLDirect,
	half3 normal, half3 viewDir,
	UnityLight light, UnityIndirect gi)
{
	half perceptualRoughness = SmoothnessToPerceptualRoughness_AFS (smoothness);
	half3 halfDir = Unity_SafeNormalize (light.dir + viewDir);

// NdotV should not be negative for visible pixels, but it can happen due to perspective projection and normal mapping
// In this case normal should be modified to become valid (i.e facing camera) and not cause weird artifacts.
// but this operation adds few ALU and users may not want it. Alternative is to simply take the abs of NdotV (less correct but works too).
// Following define allow to control this. Set it to 0 if ALU is critical on your platform.
// This correction is interesting for GGX with SmithJoint visibility function because artifacts are more visible in this case due to highlight edge of rough surface
// Edit: Disable this code by default for now as it is not compatible with two sided lighting used in SpeedTree.

#if !defined(UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV)
	#define UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV 0
#endif

#if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV
	// The amount we shift the normal toward the view vector is defined by the dot product.
	half shiftAmount = dot(normal, viewDir);
	normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f) : normal;
	// A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
	//normal = normalize(normal);

	half nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
#else
	half nv = abs(dot(normal, viewDir));	// This abs allow to limit artifact
#endif

	half nl = NdotLDirect; //saturate(dot(normal, light.dir));
	half nh = saturate(dot(normal, halfDir));

	half lv = saturate(dot(light.dir, viewDir));
	half lh = saturate(dot(light.dir, halfDir));

	// Diffuse term
	half diffuseTerm = DisneyDiffuse_AFS(nv, nl, lh, perceptualRoughness) * nl;

	// Specular term
	// HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
	// BUT 1) that will make shader look significantly darker than Legacy ones
	// and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
	half roughness = PerceptualRoughnessToRoughness_AFS(perceptualRoughness);
#if UNITY_BRDF_GGX
	half V = SmithJointGGXVisibilityTerm_AFS(nl, nv, roughness);
	half D = GGXTerm_AFS(nh, roughness);
#else
	// Legacy
	half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);
	half D = NDFBlinnPhongNormalizedTerm (nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
#endif

	half specularTerm = V*D * UNITY_PI; // Torrance-Sparrow model, Fresnel is applied later

#	ifdef UNITY_COLORSPACE_GAMMA
		specularTerm = sqrt(max(1e-4h, specularTerm));
#	endif

	// specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
	#if SHADER_API_METAL
		specularTerm = max(0, specularTerm * nl);
	#else
		specularTerm = specularTerm * nl;
	#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
	specularTerm = 0.0;
#endif

	// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
	half surfaceReduction;
#	ifdef UNITY_COLORSPACE_GAMMA
		surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
#	else
		surfaceReduction = 1.0 / (roughness*roughness + 1.0);			// fade \in [0.5;1]
#	endif

	// To provide true Lambert lighting, we need to be able to kill specular completely.
	specularTerm *= any(specColor) ? 1.0 : 0.0;

	half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));
    half3 color =	diffColor * (gi.diffuse + light.color * diffuseTerm)
                    + specularTerm * light.color * FresnelTerm (specColor, lh)
					+ surfaceReduction * gi.specular * FresnelLerp (specColor, grazingTerm, nv);

	return half4(color, 1);
}



#endif // AFS_BRDF_INCLUDED
