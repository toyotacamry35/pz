using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem;
using Newtonsoft.Json;

namespace Assets.Src.ResourcesSystem.Collections
{
    //This should be used for things where the K is a struct of some kind (even a resource)
    [JsonArray]
    public class ResMap<K, V> : Dictionary<K, ResourceRef<V>> where K : struct where V : BaseResource
    {

    }

    //K is IConvertible due to the advice of: https://stackoverflow.com/a/8746643
    //My intent is to restrict ResDictionary to only primitive valuetypes
    public class ResDictionary<K, V> : Dictionary<K, ResourceRef<V>> where K : IConvertible where V : BaseResource
    {

    }
}