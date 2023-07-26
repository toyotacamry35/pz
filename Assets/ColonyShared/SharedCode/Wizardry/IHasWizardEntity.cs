using SharedCode.EntitySystem;
using SharedCode.Refs;

namespace SharedCode.Wizardry
{
    public interface IHasWizardEntity
    {
        EntityRef<IWizardEntity> Wizard { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ISlaveWizardHolder SlaveWizardHolder { get; set; }
    }
    
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ISlaveWizardHolder : IDeltaObject
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull)]
        ISpellDoer SpellDoer { get; }  
    }
}
