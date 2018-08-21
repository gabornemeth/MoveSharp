using System.Text;
using System.Xml;

namespace MoveSharp.Strippers
{
    /// <summary>
    /// Activity stripper for TCX files
    /// </summary>
    public class TcxStripper : IStripper
    {
        public void Strip(System.IO.Stream input, System.IO.Stream output, StripOptions options)
        {
            bool writeEnabled = true; // controls whether the active readed element has to be written to the result

            using (XmlReader reader = XmlReader.Create(input))
            {
                var writerSettings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false) // don't write BOM in the first byte
                };
                using (XmlWriter writer = XmlWriter.Create(output, writerSettings))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (!writeEnabled)
                                    break;
                                if (((options & StripOptions.Power) != StripOptions.None && reader.Name == "Watts") ||
                                    ((options & StripOptions.HeartRate) != StripOptions.None && reader.Name == "HeartRateBpm") ||
                                    ((options & StripOptions.Cadence) != StripOptions.None && reader.Name == "Cadence"))
                                {
                                    // skip elements we want to strip
                                    writeEnabled = false;
                                    break;
                                }
                                writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                                writer.WriteAttributes(reader, true);
                                if (reader.IsEmptyElement)
                                {
                                    writer.WriteEndElement();
                                }
                                break;
                            case XmlNodeType.Text:
                                if (!writeEnabled)
                                    break;
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.Whitespace:
                            case XmlNodeType.SignificantWhitespace:
                                writer.WriteWhitespace(reader.Value);
                                break;
                            case XmlNodeType.CDATA:
                                writer.WriteCData(reader.Value);
                                break;
                            case XmlNodeType.EntityReference:
                                writer.WriteEntityRef(reader.Name);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.DocumentType:
                                writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                if (writeEnabled)
                                    writer.WriteFullEndElement();
                                if (((options & StripOptions.Power) != StripOptions.None && reader.Name == "Watts") ||
                                    ((options & StripOptions.HeartRate) != StripOptions.None && reader.Name == "HeartRateBpm") ||
                                    ((options & StripOptions.Cadence) != StripOptions.None && reader.Name == "Cadence"))
                                    writeEnabled = true; // nem kell a következőket kihagyni
                                break;
                        }
                    }
                }
            }
        }
    }
}
