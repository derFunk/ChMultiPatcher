using System.Collections.Generic;
using ChMultiPatcher.Data;

namespace ChMultiPatcher.PatchSources
{
    interface IPatchSource
    {
        IEnumerable<Patch> GetPatchesFromSource();
    }
}
