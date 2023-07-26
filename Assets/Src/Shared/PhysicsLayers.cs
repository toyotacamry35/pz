using UnityEngine;

namespace Assets.Src.Shared
{
    public static class PhysicsLayers
    {
        //TODO: make some automatic system
        public static int Default                       { get; } = 0;
        public static int WorldCell                     { get; } = LayerMask.NameToLayer("WorldCell");
        public static int Terrain                       { get; } = LayerMask.NameToLayer("Terrain");
        //
        //
        public static int Trigger                       { get; } = LayerMask.NameToLayer("Trigger");
        public static int Destructables                 { get; } = LayerMask.NameToLayer("Destructables");
        public static int DestructChunks                { get; } = LayerMask.NameToLayer("DestructChunks"); // Destructable object is present by set of destruct chunks when destruction 've been started
        public static int DetachedDestructChunks        { get; } = LayerMask.NameToLayer("DetachedDestructChunks"); // Change "DestractChanks" to this OnDetachFromObject to prevent receiving hits ('cos it can't pass logic to parent any more)
        public static int Interactive                   { get; } = LayerMask.NameToLayer("Interactive");
        public static int Vision                        { get; } = LayerMask.NameToLayer("Vision");
        public static int Active                        { get; } = LayerMask.NameToLayer("Active");
        public static int Grass                         { get; } = LayerMask.NameToLayer("Grass");
        public static int Motion                        { get; } = LayerMask.NameToLayer("Motion");
        public static int ReplicationRangeCheck         { get; } = LayerMask.NameToLayer("ReplicationRangeCheck");
        public static int Sound                         { get; } = LayerMask.NameToLayer("Sound");
        public static int Bullet                        { get; } = LayerMask.NameToLayer("Bullet");
        public static int CharacterController           { get; } = LayerMask.NameToLayer("CharacterController");
        public static int DefaultMask                   { get; } = 1 << Default;                //  0
        public static int TerrainMask                   { get; } = 1 << Terrain;                //  9
        //                                                                                      // 10            
        //                                                                                      // 11
        //                                                                                      // 12
        public static int DestructablesMask             { get; } = 1 << Destructables;          // 14
        public static int DestructChunksMask            { get; } = 1 << DestructChunks;         // 15
        public static int DetachedDestructChunksMask    { get; } = 1 << DetachedDestructChunks; // 16
        public static int InteractiveMask               { get; } = 1 << Interactive;            // 17
        public static int ActiveMask                    { get; } = 1 << Active;                 // 19
        public static int BulletMask                    { get; } = 1 << Bullet;                 // 25
        public static int CharacterControllerMask       { get; } = 1 << CharacterController;                 
        
        public static int InteractiveAndDestrMask       { get; } = InteractiveMask | DestructablesMask;
        public static int CheckIsGroundedMask           { get; } = DefaultMask | TerrainMask | DestructablesMask | DestructChunksMask;
        public static int PlaceBuildElementMask         { get; } = DefaultMask | TerrainMask | InteractiveMask | DestructablesMask | DestructChunksMask;
        /// <summary>
        /// Слой для объектов по которым игрок может быть атакой 
        /// </summary>
        public static int AttackMask                    { get; } = DefaultMask | DestructablesMask | ActiveMask;
        public static int TargetableMask                { get; } = DestructablesMask | InteractiveMask | ActiveMask | CharacterControllerMask;
        public static int CheckIsGroundedAndWaterMask   { get; } = CheckIsGroundedMask; //добавить воду, когда у нее появится коллайдер
        public static int BoxSpawnMask                  { get; } = CheckIsGroundedMask | InteractiveAndDestrMask;
        public static int BulletMaskEx                  { get; } = BulletMask | CheckIsGroundedMask | InteractiveAndDestrMask | ActiveMask;
        public static int BuildMask                     { get; } = DefaultMask | CheckIsGroundedMask | InteractiveMask;
        public static int ObstacleMask                  { get; } = TerrainMask;
    }
}
