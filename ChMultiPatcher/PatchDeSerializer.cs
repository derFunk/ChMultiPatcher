using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using ChMultiPatcher.Data;
using System.IO;

namespace ChMultiPatcher
{
    public interface IPatchDeSerializer
    {
        Patch Deserialize(Stream stream);
        void SerializeIntoStream(Patch patch, Stream stream);
    }

    /// <summary>
    /// Central class to handle serialization and deserialization.
    /// If you want another de-/serializer, return it here!
    /// </summary>
    public class PatchDeSerializerService
    {
        private static IPatchDeSerializer m_patchDeSerializer;

        public static IPatchDeSerializer GetPatchDeSerializer()
        {
            return m_patchDeSerializer ?? (m_patchDeSerializer = new BinaryPatchDeSerializer());
        }
    }

    /// <summary>
    /// De-/Serializes in XML Format. Bigger than binary
    /// </summary>
    public class XmlPatchDeSerializer : IPatchDeSerializer
    {
        private readonly XmlSerializer m_xmlSer;

        public XmlPatchDeSerializer()
        {
            m_xmlSer = new XmlSerializer(typeof(Patch));
        }

        public Patch Deserialize(Stream stream)
        {
            return (Patch)m_xmlSer.Deserialize(stream);
        }

        public void SerializeIntoStream(Patch patch, Stream stream)
        {
            m_xmlSer.Serialize(stream, patch);
        }
    }

    /// <summary>
    /// De-/Serializes binary. Care has been taken since it normally needs the same assembly 
    /// to serialize and deserialize it. Thus we have a special binder which allows us to
    /// deserialize a binary blob even in another assembly as it was created in.
    /// </summary>
    public class BinaryPatchDeSerializer : IPatchDeSerializer
    {
        private readonly BinaryFormatter m_bFormatter;

        public BinaryPatchDeSerializer()
        {
            m_bFormatter = new BinaryFormatter();
            m_bFormatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
        }

        public Patch Deserialize(Stream stream)
        {
           return (Patch)m_bFormatter.Deserialize(stream);
        }

        public void SerializeIntoStream(Patch patch, Stream stream)
        {
            m_bFormatter.Serialize(stream, patch);
        }
    }

    sealed class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            //// rename all to the same assembly.

            //typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName.Replace("PatchCreator", "ChMultiPatcher")));
            
            //return typeToDeserialize;
            ///*
            

            // dont do magic assembly remove stuff at system namespace!
            if (typeName.StartsWith("System."))
            {
                typeToDeserialize = Type.GetType(typeName);
                return typeToDeserialize;
            }
            
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;

            // In this case we are always using the current assembly
            assemblyName = currentAssembly;

            // Get the type using the typeName and assemblyName
            typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
             //* */
        }
    }
}
