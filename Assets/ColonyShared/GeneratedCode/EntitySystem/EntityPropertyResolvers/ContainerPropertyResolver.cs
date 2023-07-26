//using SharedCode.Entities;
//using SharedCode.Logging;
//using SharedCode.Entities.Engine;

//namespace SharedCode.EntitySystem.EntityPropertyResolvers
//{
//    public class ContainerPropertyResolver: IEntityPropertyResolver
//    {
//        public ContainerPropertyResolver()
//        {
//        }

//        public T Resolve<T>(IEntity entity, PropertyAddress address)
//        {
//            var character = entity as IWorldCharacter;
//            if (character != null)
//            {
//                switch (address.Address)
//                {
//                    case 1: return (T)character.Inventory;
//                    case 2: return (T)character.Doll;
//                }
//            }

//            var worldBox = entity as IWorldBox;
//            if (worldBox != null)
//            {
//                switch (address.Address)
//                {
//                    case 3: return (T)worldBox.Inventory;
//                }
//            }

//            var worldBuilding = entity as IWorldBuilding;
//            if (worldBuilding != null)
//            {
//                switch (address.Address)
//                {
//                    case 4: return (T)worldBuilding.Inventory;
//                }
//            }

//            var craftEngine = entity as ICraftEngine;
//            if (craftEngine != null)
//            {
//                switch (address.Address)
//                {
//                    case 5: return (T)craftEngine.CraftContainer;
//                }
//            }

//            Log.Logger.IfError()?.Message("ContainerPropertyResolver Resolve: Incorrect property address {1} entity {0}", entity.Id, address.Address).Write();
//            return default(T);
//        }

//        public int GetPropertyAddress(IEntity entity, object property)
//        {
//            var character = entity as IWorldCharacter;
//            if (character != null)
//            {
//                if (property == character.Inventory)
//                    return 1;
//                if (property == character.Doll)
//                    return 2;
//            }

//            var worldBox = entity as IWorldBox;
//            if (worldBox != null)
//            {
//                if (property == worldBox.Inventory)
//                    return 3;
//            }

//            var worldBuilding = entity as IWorldBuilding;
//            if (worldBuilding != null)
//            {
//                if (property == worldBuilding.Inventory)
//                    return 4;
//            }

//            var craftEngine = entity as ICraftEngine;
//            if (craftEngine != null)
//            {
//                if (property == craftEngine.CraftContainer)
//                    return 5;
//            }

//            Log.Logger.IfError()?.Message("ContainerPropertyResolver GetPropertyAddress: Incorrect property {1} entity {0}", entity.Id, property.GetType().Name).Write();
//            return -1;
//        }
//    }
//}
