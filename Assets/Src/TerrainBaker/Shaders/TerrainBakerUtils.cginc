
//optimize late

#define NO_MATERIAL_TRH		(254.5 / 255.0)
#define ALPHA_MAP_TRH		(0.9 / 255.0)

//#define FORCE_SET_ALPHA_CHANNEL_FOR_DEBUG


void AddUnique(inout float4 current, float mat)
{
	if (current.r > NO_MATERIAL_TRH || current.r == mat)
	{
		current.r = mat;
	}
	else if (current.g > NO_MATERIAL_TRH || current.g == mat)
	{
		current.g = mat;
	}
	else if (current.b > NO_MATERIAL_TRH || current.b == mat)
	{
		current.b = mat;
	}
	else if (current.a > NO_MATERIAL_TRH || current.a == mat)
	{
		current.a = mat;
	}
}

#ifdef COLLECTION_SIZE

void ResetCollection(inout float2 collection[COLLECTION_SIZE])
{
	for (int i = 0; i < COLLECTION_SIZE; i++)
	{
		collection[i] = float2(1, 0);
	}
}

void AddUnique(inout float2 collection[COLLECTION_SIZE], float mat, float sortWeight)
{
	if (mat > NO_MATERIAL_TRH) sortWeight = 0;
	int index = -1;
	[unroll]
	for (int i = 0; i < COLLECTION_SIZE; i++)
	{
		if (collection[i].x == mat) index = i;
		if (collection[i].y == 0 && index == -1) index = i;
	}
	index = max(index, 0);
	collection[index].x = mat;
	collection[index].y += sortWeight;
}

void SortCollection(inout float2 collection[COLLECTION_SIZE])
{
	[unroll]
	for (int i = 0; i < COLLECTION_SIZE - 1; i++)
	{
		[unroll]
		for (int j = i + 1; j < COLLECTION_SIZE; j++)
		{
			if (collection[j].y > collection[i].y)
			{
				float2 tmp = collection[i];
				collection[i] = collection[j];
				collection[j] = tmp;
			}
		}
	}
}

#endif
