// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutTestEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _TestDeltaListInt.Clear();
                var TestDeltaListIntitem0__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem0__rnd);
                var TestDeltaListIntitem1__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem1__rnd);
                var TestDeltaListIntitem2__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem2__rnd);
                var TestDeltaListIntitem3__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem3__rnd);
                var TestDeltaListIntitem4__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem4__rnd);
                var TestDeltaListIntitem5__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem5__rnd);
                var TestDeltaListIntitem6__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem6__rnd);
                var TestDeltaListIntitem7__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem7__rnd);
                var TestDeltaListIntitem8__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem8__rnd);
                var TestDeltaListIntitem9__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem9__rnd);
            }

            {
                _TestDeltaListDeltaObject.Clear();
                var TestDeltaListDeltaObjectitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem0__rnd);
            }

            {
                _TestDeltaDictionaryInt.Clear();
                var TestDeltaDictionaryIntitem0__rnd = (int)__random__.Next(100);
                int __key0__ = 0;
                TestDeltaDictionaryInt[__key0__] = TestDeltaDictionaryIntitem0__rnd;
                var TestDeltaDictionaryIntitem1__rnd = (int)__random__.Next(100);
                int __key1__ = 1;
                TestDeltaDictionaryInt[__key1__] = TestDeltaDictionaryIntitem1__rnd;
                var TestDeltaDictionaryIntitem2__rnd = (int)__random__.Next(100);
                int __key2__ = 2;
                TestDeltaDictionaryInt[__key2__] = TestDeltaDictionaryIntitem2__rnd;
                var TestDeltaDictionaryIntitem3__rnd = (int)__random__.Next(100);
                int __key3__ = 3;
                TestDeltaDictionaryInt[__key3__] = TestDeltaDictionaryIntitem3__rnd;
                var TestDeltaDictionaryIntitem4__rnd = (int)__random__.Next(100);
                int __key4__ = 4;
                TestDeltaDictionaryInt[__key4__] = TestDeltaDictionaryIntitem4__rnd;
                var TestDeltaDictionaryIntitem5__rnd = (int)__random__.Next(100);
                int __key5__ = 5;
                TestDeltaDictionaryInt[__key5__] = TestDeltaDictionaryIntitem5__rnd;
                var TestDeltaDictionaryIntitem6__rnd = (int)__random__.Next(100);
                int __key6__ = 6;
                TestDeltaDictionaryInt[__key6__] = TestDeltaDictionaryIntitem6__rnd;
            }

            {
                _TestDeltaDictionaryIntDeltaObject.Clear();
                var TestDeltaDictionaryIntDeltaObjectitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key0__ = 0;
                TestDeltaDictionaryIntDeltaObject[__key0__] = TestDeltaDictionaryIntDeltaObjectitem0__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key1__ = 1;
                TestDeltaDictionaryIntDeltaObject[__key1__] = TestDeltaDictionaryIntDeltaObjectitem1__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key2__ = 2;
                TestDeltaDictionaryIntDeltaObject[__key2__] = TestDeltaDictionaryIntDeltaObjectitem2__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key3__ = 3;
                TestDeltaDictionaryIntDeltaObject[__key3__] = TestDeltaDictionaryIntDeltaObjectitem3__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key4__ = 4;
                TestDeltaDictionaryIntDeltaObject[__key4__] = TestDeltaDictionaryIntDeltaObjectitem4__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key5__ = 5;
                TestDeltaDictionaryIntDeltaObject[__key5__] = TestDeltaDictionaryIntDeltaObjectitem5__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key6__ = 6;
                TestDeltaDictionaryIntDeltaObject[__key6__] = TestDeltaDictionaryIntDeltaObjectitem6__rnd;
            }

            {
                var TestProperty__rnd = (int)__random__.Next(100);
                TestProperty = TestProperty__rnd;
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TestDeltaObject
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var Value__rnd = (int)__random__.Next(100);
                Value = Value__rnd;
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutSubTestEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _TestDeltaListInt.Clear();
                var TestDeltaListIntitem0__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem0__rnd);
                var TestDeltaListIntitem1__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem1__rnd);
                var TestDeltaListIntitem2__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem2__rnd);
                var TestDeltaListIntitem3__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem3__rnd);
                var TestDeltaListIntitem4__rnd = (int)__random__.Next(100);
                TestDeltaListInt.Add(TestDeltaListIntitem4__rnd);
            }

            {
                _TestDeltaListDeltaObject.Clear();
                var TestDeltaListDeltaObjectitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem0__rnd);
                var TestDeltaListDeltaObjectitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem1__rnd);
                var TestDeltaListDeltaObjectitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem2__rnd);
                var TestDeltaListDeltaObjectitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem3__rnd);
                var TestDeltaListDeltaObjectitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem4__rnd);
                var TestDeltaListDeltaObjectitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem5__rnd);
                var TestDeltaListDeltaObjectitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaListDeltaObjectitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                TestDeltaListDeltaObject.Add(TestDeltaListDeltaObjectitem6__rnd);
            }

            {
                _TestDeltaDictionaryInt.Clear();
                var TestDeltaDictionaryIntitem0__rnd = (int)__random__.Next(100);
                int __key0__ = 0;
                TestDeltaDictionaryInt[__key0__] = TestDeltaDictionaryIntitem0__rnd;
                var TestDeltaDictionaryIntitem1__rnd = (int)__random__.Next(100);
                int __key1__ = 1;
                TestDeltaDictionaryInt[__key1__] = TestDeltaDictionaryIntitem1__rnd;
                var TestDeltaDictionaryIntitem2__rnd = (int)__random__.Next(100);
                int __key2__ = 2;
                TestDeltaDictionaryInt[__key2__] = TestDeltaDictionaryIntitem2__rnd;
            }

            {
                _TestDeltaDictionaryIntDeltaObject.Clear();
                var TestDeltaDictionaryIntDeltaObjectitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key0__ = 0;
                TestDeltaDictionaryIntDeltaObject[__key0__] = TestDeltaDictionaryIntDeltaObjectitem0__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key1__ = 1;
                TestDeltaDictionaryIntDeltaObject[__key1__] = TestDeltaDictionaryIntDeltaObjectitem1__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key2__ = 2;
                TestDeltaDictionaryIntDeltaObject[__key2__] = TestDeltaDictionaryIntDeltaObjectitem2__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key3__ = 3;
                TestDeltaDictionaryIntDeltaObject[__key3__] = TestDeltaDictionaryIntDeltaObjectitem3__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key4__ = 4;
                TestDeltaDictionaryIntDeltaObject[__key4__] = TestDeltaDictionaryIntDeltaObjectitem4__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key5__ = 5;
                TestDeltaDictionaryIntDeltaObject[__key5__] = TestDeltaDictionaryIntDeltaObjectitem5__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key6__ = 6;
                TestDeltaDictionaryIntDeltaObject[__key6__] = TestDeltaDictionaryIntDeltaObjectitem6__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem7__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem7__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key7__ = 7;
                TestDeltaDictionaryIntDeltaObject[__key7__] = TestDeltaDictionaryIntDeltaObjectitem7__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem8__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem8__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key8__ = 8;
                TestDeltaDictionaryIntDeltaObject[__key8__] = TestDeltaDictionaryIntDeltaObjectitem8__rnd;
                var TestDeltaDictionaryIntDeltaObjectitem9__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.TestDeltaObject>();
                ((ResourcesSystem.Base.IHasRandomFill)TestDeltaDictionaryIntDeltaObjectitem9__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                int __key9__ = 9;
                TestDeltaDictionaryIntDeltaObject[__key9__] = TestDeltaDictionaryIntDeltaObjectitem9__rnd;
            }

            {
                var TestProperty__rnd = (int)__random__.Next(100);
                TestProperty = TestProperty__rnd;
            }
        }
    }
}