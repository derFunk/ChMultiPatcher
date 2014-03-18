using System.Collections.Generic;
using ChMultiPatcher.Data;
using ChMultiPatcher.PatchSources;

namespace ChMultiPatcher.PatchRepositories
{
    class PatchRepository : IPatchRepository
    {
        private readonly List<Patch> m_availablePatches = new List<Patch>();


        public void AddPatch(Patch p)
        {
            m_availablePatches.Add(p);
        }


        public void AddPatchesFromSource(IPatchSource patchSource)
        {
            m_availablePatches.AddRange(patchSource.GetPatchesFromSource());
        }

        public List<Patch> GetAvailablePatches()
        {
            return m_availablePatches;
        }
    }
}
