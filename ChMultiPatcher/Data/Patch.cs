using System;
using System.Collections;
using System.Xml.Serialization;

namespace ChMultiPatcher.Data
{
    [Serializable]
    public class Patch
    {
        /// <summary>
        /// Patches from this revision number
        /// </summary>
        /// <value>
        /// From rev.
        /// </value>
        public string FromRev { get; set; }

        /// <summary>
        /// Patches to this revision number
        /// </summary>
        /// <value>
        /// To rev.
        /// </value>
        public string ToRev { get; set; }

        /// <summary>
        /// The patch's name or the project's name which should be patched
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        // Dont use generic list, as the binary serializer didn't get it...
        public ArrayList FileDiffs { get; set; }

        /// <summary>
        /// Stripped path names of the directories which can be removed, since they weren't
        /// on the To-Revision of the patch.
        /// </summary>
        /// <value>
        /// The directories to remove.
        /// </value>
        public ArrayList DirectoriesToRemove { get; set; }

        /// <summary>
        /// Fingerprint of the revision FromRev which shall be patched with 
        /// this patch. If fingerprints don't match at patch application,
        /// patch is not applicable
        /// </summary>
        //[XmlAttribute]
        //public string FromRevFingerprint { get; set; }

        /// <summary>
        /// Parameterless constructor needed for de/serialization
        /// </summary>
        public Patch()
        {  
        }

        public Patch(string fromRev, string toRev)
            : this()
        {
            FromRev = fromRev;
            ToRev = toRev;
            FileDiffs = new ArrayList();
        }
    }
}
