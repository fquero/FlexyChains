using FlexyChains_Library.Interfaces;
using FlexyChains_Library.ProtectionProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library
{
    public class GenericNodeManipulator : NodeBase
    {
        
        public GenericNodeManipulator(
            string nodeName,
            string elementName
            ) : base(nodeName, elementName)
        {
            ProtectionProvider = new RsaProtector();
        }

        //public override void DecryptNode()
        //{
        //    try
        //    {
        //        // Desencriptamos y obtenemos el nodo desencriptado.
        //        XmlNode decryptedNode = ProtectionProvider.Desencrypt(ParentNode);

        //        // Verificamos si el nodo desencriptado pertenece al mismo documento.
        //        // Si no es así, lo importamos al documento original.
        //        if (decryptedNode.OwnerDocument != XmlDocument)
        //        {
        //            decryptedNode = XmlDocument.ImportNode(decryptedNode, true);
        //        }

        //        // Asignamos el nodo desencriptado (ya importado) como nuevo ParentNode.
        //        ParentNode = decryptedNode;
        //        ChildNodesList = ParentNode.SelectNodes(_childNodeName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public override void DecryptNode()
        {
            try
            {
                // 1. Obtener el nodo desencriptado (y si es de otro doc, importarlo).
                XmlNode decryptedNode = ProtectionProvider.Desencrypt(ParentNode);
                if (decryptedNode.OwnerDocument != XmlDocument)
                {
                    decryptedNode = XmlDocument.ImportNode(decryptedNode, true);
                }

                // 2. Sobrescribir el contenido de ParentNode sin reemplazar la instancia.
                //    a) Eliminar atributos y nodos hijos actuales
                ParentNode.RemoveAll(); // Quita hijos y atributos
                                        //    b) Copiar atributos desde el nodo desencriptado
                foreach (XmlAttribute attr in decryptedNode.Attributes)
                {
                    // Agregar atributo al nodo original
                    ((XmlElement)ParentNode).SetAttribute(attr.Name, attr.Value);
                }
                //    c) Copiar cada hijo desde el nodo desencriptado
                foreach (XmlNode child in decryptedNode.ChildNodes)
                {
                    ParentNode.AppendChild(child.CloneNode(deep: true));
                }

                // 3. ChildNodesList sigue apuntando al mismo ParentNode, 
                //    solo necesitas refrescar su contenido.
                //ChildNodesList = ParentNode.SelectNodes(_childNodeName);

                //Si se ha definido un nodo hijo, se actualiza la lista de nodos hijos.
                if (_childNodeName != null)
                {
                    try
                    {
                        ChildNodesList = ParentNode.SelectNodes(_childNodeName);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public XElement EditItem(XElement item)
        {
            throw new NotImplementedException();
        }


        

    }
}
