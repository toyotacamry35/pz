enum ShaderVariantsStripperOrder
{
    Package = 1,                // Used to strip the invalid shader variants of a package / render pipeline
    BuildConfiguration = 2,     // Used to strip debug/release specific shader variants
    Project = 3,                // Used to strip the unused features of the included framework
    Log = 4                     // Used to write the shader variants sstripping scripts and remove all shader variants build for quicker iteration time
}
