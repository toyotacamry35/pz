using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourceIndexer.Validator
{
    public static class ValidatorCollection
    {
        public delegate IEnumerable<string> ValidateHandler(IEnumerable<IResource> newResources);

        public static IEnumerable<string> DuplicatesValidation(IEnumerable<IResource> newResources)
        {
            var duplicates = GetDuplicateResources(newResources);
            return GetDuplicatesIdsErrors(duplicates);
        }

        public static IEnumerable<string> GetDuplicatesIdsErrors(IEnumerable<(Guid Id, IResource[] Resources)> duplicates)
        {
            List<string> str = new List<string>();
            if (duplicates.Any())
            {
                foreach (var duplicate in duplicates)
                    str.Add($"Dublicates Key [{duplicate.Id}] in: " + string.Join(", ", duplicate.Resources.Select(v => v.Address.ToString())));
            }

            return str;
        }

        public static IEnumerable<(Guid Id, IResource[] Resources)> GetDuplicateResources(IEnumerable<IResource> newResources)
        {
            IEnumerable<(Guid Id, IResource[] Resources)> duplicates =
                (newResources ?? Enumerable.Empty<IResource>())
                .Where(v => v is ISaveableResource)
                .Where(v => v.Address.ProtoIndex == 0)
                .Where(v => ((ISaveableResource)v).Id != Guid.Empty)
                .GroupBy(v => ((ISaveableResource)v).Id)
                .Where(x => x.Skip(1).Any())
                .Select(v => (v.Key, Resources: v.ToArray()));

            return duplicates;
        }

        public static IEnumerable<string> InvalidValidation(IEnumerable<IResource> newResources)
        {
            var invalidIdResources = GetInvalidResources(newResources);
            return GetInvalidIdErrors(invalidIdResources);
        }

        public static IEnumerable<string> GetInvalidIdErrors(IEnumerable<IResource> invalidResources)
        {
            return invalidResources.Select(v => "Invalid ID in: " + v.Address.ToString());
        }

        public static IEnumerable<IResource> GetInvalidResources(IEnumerable<IResource> newResources)
        {
            var invalidIdResources = (newResources ?? Enumerable.Empty<IResource>())
                .Where(v => v != null && v is ISaveableResource && ((ISaveableResource)v).Id == Guid.Empty).ToArray();

            return invalidIdResources;
        }
    }
}
