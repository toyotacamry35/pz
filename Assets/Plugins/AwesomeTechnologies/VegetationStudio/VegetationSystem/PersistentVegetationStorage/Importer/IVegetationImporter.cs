namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    #if PERSISTENT_VEGETATION
    public interface IVegetationImporter
    {
        string ImporterName { get;}
        PersistentVegetationStoragePackage PersistentVegetationStoragePackage { get; set; }
        VegetationPackage VegetationPackage { get; set; }
        PersistentVegetationStorage PersistentVegetationStorage { get; set; }
        void OnGUI();
    }
#endif
}
