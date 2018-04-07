using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Schema;

namespace CalculationParser
{

	public enum Operand {
		NONE,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE
	};


	[XmlRoot("folder")]
	public class Calculations {
		[XmlElement ("folder")] public List<Calculation> calculations = new List<Calculation> ();
	}

	public class Calculation:IXmlSerializable {
		public int[] hasElement { get; set; }
		public string uid { get; set; }
		public Operand operand { get; set; }
		public int mod { get; set; }

		public XmlSchema GetSchema () {return null;}
		public void ReadXml (XmlReader reader) {
			hasElement = new int[] {0 ,0, 0, 0};
			reader.ReadStartElement ();
			while (true) {
				reader.MoveToContent ();
				if (reader.NodeType != XmlNodeType.Element)
					break;
				switch (reader.GetAttribute ("name")) {
				case "uid":
					++hasElement [0];
					uid = reader.GetAttribute ("value");
					break;
				case "operand":
					++hasElement [1];
					operand = string2operand(reader.GetAttribute ("value"));
					break;
				case "mod":
					if (int.TryParse (reader.GetAttribute ("value"))) {
						mod = int.Parse (reader.GetAttribute ("value"));
						++hasElement [2];
					}
					break;
				default:
					++hasElement [3];
					break;
				}
				reader.ReadElementContentAsString ();
			}
			reader.ReadEndElement ();
		}
		public void WriteXml(XmlWriter writer) {}
		private Operand string2operand (string str) {
			switch (str) {
			case "add":
				return Operand.ADD;
			case "subtract":
				return Operand.SUBTRACT;
			case "multiply":
				return Operand.MULTIPLY;
			case "divide":
				return Operand.DIVIDE;
			default:
				return Operand.NONE;
			}
		}
	}

}

