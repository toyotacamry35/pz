using UnityEngine;

[SelectionBase]
public class WaterfallHelper : MonoBehaviour
{
    [Header("Изменение ширины")]
    [SerializeField]
    private float currentWidth = 1f;

    [Header("Изменение высоты")]
    [SerializeField]
    private float currentHeight = 1f;

    [Header("Изменение уклона")]
    [SerializeField]
    private float currentBend = 1f;
    [Header("Корекция пола высота")]
    [SerializeField]
    private float currentFloorheight = 0f;
    [Header("Коррекция пола вынос")]
    [SerializeField]
    private float currentFloorwidth = 0f;

    [Header("Продвинутые настройки VFX", order = 0)]
    
    [SerializeField]
    private LayerMask postProcessParticles = 1<<26;
    
    [Header("Линки объектов", order = 1)]
    [SerializeField]
    private ParticleSystem WaterfallUp;
    [SerializeField]
    private ParticleSystem WaterVapor;
    [SerializeField]
    private ParticleSystem WaterFront;
    [SerializeField]
    private ParticleSystem WaterFlow;
    [SerializeField]
    private ParticleSystem WaterFlow2;
    [SerializeField]
    private ParticleSystem WaterFlow3;
    [SerializeField]
    private ParticleSystem WaterFrontUp;

    [SerializeField]
    private GameObject Floor;

   [SerializeField]
    private ParticleSystem WaterMist;
    [SerializeField]
    private ParticleSystem WaterSplash;
    [SerializeField]
    private ParticleSystem FloorHit;

    //LOD1
    [SerializeField]
    private ParticleSystem LOD1Vapor;

    [SerializeField]
    private ParticleSystem LOD1Front;

    [SerializeField]
    private ParticleSystem LOD1Mist;

    //LOD2
    [SerializeField]
    private ParticleSystem LOD2Vapor;


    [Header("Коэффициенты ширины")]
    [SerializeField]
    private float waterfallUpRateOverTime = 0.9f;

    [SerializeField]
    private float waterfallUpShapeScale = 0.45f;
    [SerializeField]
    private float waterfallUpShapeScaleSmall = 0.3f;
    [SerializeField]
    private float waterfallUpShapeScaleOut = -0.44f;

    [Header("Коэффициенты высоты")]
    [SerializeField]
    private float waterfallUpStartLifetime = 0.105f;

    [SerializeField]
    private float waterVaporStartLifetime = 0.105f;

    [SerializeField]
    private float waterFrontStartLifetime = 0.11f;


    [SerializeField]
    private float floorPositionRelative = 1f;

    private Vector3 _prevScale;

    void OnValidate()
    {
        ApplyProperties();
    }

    public void ApplyProperties()
    {
        /*var objects = gameObject.GetComponentsInChildren<Transform>();
        foreach (var o in objects)
        {
            o.gameObject.layer = postProcessParticles;
        }*/
        
        //Применяем ширину;
        var emissionModule = WaterfallUp.emission;
        emissionModule.rateOverTime = waterfallUpRateOverTime * currentWidth;
        var emissionModule2 = WaterVapor.emission;
        emissionModule2.rateOverTime = 1.9f * currentWidth;
        var emissionModule3 = WaterFront.emission;
        emissionModule3.rateOverTime = 0.99f * currentWidth;

        var emissionModuleFrontUp = WaterFrontUp.emission; //will use only if height more than 60
        // emissionModuleFrontUp.rateOverTime = 0.99f * currentWidth;

        var emissionModule4 = WaterFlow.emission;
        emissionModule4.rateOverTime = 1.1f * currentWidth + 2f;
        var emissionModule5 = WaterFlow2.emission;
        emissionModule5.rateOverTime = Mathf.Abs(1.1f * (currentWidth - 3f)) + 1f;
        var emissionModule6 = WaterFlow3.emission;
        emissionModule6.rateOverTime = 1.1f * currentWidth + 2f;

        var emissionModuleMist = WaterMist.emission;
        emissionModuleMist.rateOverTime = 0.59f * currentWidth;


        var shapeModule = WaterfallUp.shape;
        shapeModule.scale = new Vector3(currentWidth * waterfallUpShapeScale, shapeModule.scale.y, shapeModule.scale.z);
        var shapeModule3 = WaterVapor.shape;
        shapeModule3.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModule.scale.y, shapeModule.scale.z);
        var shapeModule4 = WaterFront.shape;
        shapeModule4.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModule.scale.y, shapeModule.scale.z);

        var shapeModuleFrontUp = WaterFrontUp.shape;
        shapeModuleFrontUp.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModuleFrontUp.scale.y, shapeModuleFrontUp.scale.z);


        var shapeModule5 = WaterFlow.shape;
        shapeModule5.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModule5.scale.y, shapeModule5.scale.z);
        //var shapeModule6 = systemWaterFlow2.shape;
        // shapeModule6.scale = new Vector3(currentWidth * waterfallUpShapeScale, shapeModule.scale.y, shapeModule.scale.z);
        var shapeModule2 = WaterFlow3.shape;
        shapeModule2.scale = new Vector3(currentWidth * (-1.0f)*waterfallUpShapeScaleSmall, shapeModule2.scale.y, shapeModule2.scale.z);

        var shapeModule7 = WaterMist.shape;
        shapeModule7.scale = new Vector3(currentWidth * 0.45f, shapeModule7.scale.y, shapeModule7.scale.z);
        var shapeModule8 = WaterSplash.shape;
        shapeModule8.scale = new Vector3(currentWidth * 0.45f, shapeModule8.scale.y, shapeModule8.scale.z);
        var shapeModule9 = FloorHit.shape;
        shapeModule9.scale = new Vector3(currentWidth * 0.11f, shapeModule9.scale.y, shapeModule9.scale.z);

        //Применяем высоту
        var mainModule = WaterfallUp.main;
        mainModule.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f);

        var mainModuleVapor = WaterVapor.main;
        mainModuleVapor.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        var mainWaterFront = WaterFront.main;
        mainWaterFront.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        var mainWaterFlow = WaterFlow.main;
        mainWaterFlow.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        var mainWaterFlow2 = WaterFlow2.main;
        mainWaterFlow2.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;
        var mainWaterFlow3 = WaterFlow3.main;
        mainWaterFlow3.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;
        var mainWaterfrontUp = WaterFrontUp.main;


        emissionModuleFrontUp.rateOverTime = 0.99f * currentWidth * Mathf.Clamp(currentHeight - 50, 0f, 1f);


        var position = Floor.transform.localPosition;
        Floor.transform.localPosition = new Vector3(position.x, floorPositionRelative * currentHeight+ currentFloorheight,  Mathf.Sqrt(currentHeight) + 2.4f+ currentFloorwidth);

        //применяем наклон
        mainWaterFront.startSpeed = currentBend * 3.6f;
        mainModule.startSpeed = currentBend * 3.6f;

        mainModuleVapor.startSpeed = currentBend * 3.6f;

        mainWaterFlow.startSpeed = currentBend * 3.6f;

        mainWaterFlow2.startSpeed = currentBend * 3.6f;

        mainWaterFlow3.startSpeed = currentBend * 3.6f;

        mainWaterfrontUp.startSpeed = currentBend * 3.6f;

        //LOD1 на двух системах

        // ширина 
        var emissionModLOD1V = LOD1Vapor.emission;
        emissionModLOD1V.rateOverTime = 0.56f * currentWidth;
        var emissionModLOD1F = LOD1Front.emission;
        emissionModLOD1F.rateOverTime = 0.67f * currentWidth;
        var emissionModLOD1M = WaterMist.emission;
        emissionModLOD1M.rateOverTime = 0.32f * currentWidth;



        var shapeModLOD1V = LOD1Vapor.shape;
        shapeModLOD1V.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModLOD1V.scale.y, shapeModLOD1V.scale.z);
        var shapeModLOD1F = LOD1Front.shape;
        shapeModLOD1F.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModLOD1F.scale.y, shapeModLOD1F.scale.z);
        var shapeModLOD1M = WaterMist.shape;
        shapeModLOD1M.scale = new Vector3(currentWidth * 0.45f, shapeModLOD1M.scale.y, shapeModLOD1M.scale.z);



        // высота

        var mainModLOD1Vapor = LOD1Vapor.main;
        mainModLOD1Vapor.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        var mainModLOD1Front = LOD1Front.main;
        mainModLOD1Front.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        //наклон
        mainModLOD1Vapor.startSpeed = currentBend * 3.6f;

        mainModLOD1Front.startSpeed = currentBend * 3.6f;

        //LOD2 на одной системе

        // ширина 
        var emissionModLOD2V = LOD2Vapor.emission;
        emissionModLOD2V.rateOverTime = 0.62f * currentWidth;

        var shapeModLOD2V = LOD2Vapor.shape;
        shapeModLOD2V.scale = new Vector3(currentWidth * waterfallUpShapeScaleSmall, shapeModLOD2V.scale.y, shapeModLOD2V.scale.z);


        // длина

        var mainModLOD2Vapor = LOD2Vapor.main;
        mainModLOD2Vapor.startLifetime = Mathf.Sqrt(currentHeight * 2f / 9.81f) + 0.1f;

        // наклон

        mainModLOD2Vapor.startSpeed = currentBend * 3.6f;

    }
}