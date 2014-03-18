using ChMultiPatcher.Data;
using System.Collections.Generic;
using ChMultiPatcher.PatchSources;

namespace ChMultiPatcher.PatchRepositories
{
    interface IPatchRepository
    {
        List<Patch> GetAvailablePatches();

        void AddPatch(Patch p);

        void AddPatchesFromSource(IPatchSource patchSource);
    }
}
