using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CalculationParser
{

	class MainClass
	{
		public static void Main (string[] args) {
			//инициализация переменных для вывода файла с наименьшим количеством ошибок
			string bestFile = "";
			int bestFileErrors = int.MaxValue;
			//проверка на количество аргументов
			if (args.Length != 1) {
				Console.WriteLine ("Для запуска программы укажите директорию, с которой хотите работать");
				return;
			}
			//проверка директории указанной в аргументе программы
			string path = args [0]; 
			if (!Directory.Exists (path)) {
				Console.WriteLine ("Указанной директории не существует");
				return;
			}
			//работа с файлами в папке
			string[] files = Directory.GetFiles (path);
			//TODO добавить многопоточный парсинг
			foreach (string file in files) {
				//парсим только xml файлы
				if (file.Substring (file.Length - 4).ToLower () == ".xml") {
					double result = 0; //результат вычислений
					int errors = 0; //счетчик ошибок
					//десереализация xml
					XmlSerializer xmlFile = new XmlSerializer (typeof(Calculations));
					Calculations folder = new Calculations();
					using (FileStream fs = new FileStream(file, FileMode.Open)) {
						folder = (Calculations)xmlFile.Deserialize(fs);
					}
					//вычисления результата
					foreach (Calculation calculation in folder.calculations) {
						//проверка на валидность calculation 
						if (!(calculation.hasElement [0] == 1 && calculation.hasElement [1] == 1 && calculation.hasElement [2] == 1 && calculation.hasElement [3] == 0)) {
							errors++;
							//TODO добавить вывод ошибок
							continue;
						}
						//сами расчёты
						switch (calculation.operand) {
						case Operand.ADD:
							result += calculation.mod;
							break;
						case Operand.SUBTRACT:
							result -= calculation.mod;
							break;
						case Operand.MULTIPLY:
							result *= calculation.mod;
							break;
						case Operand.DIVIDE:
							//проверка делимости на нуль
							if (calculation.mod == 0) {
								++errors;
								break;
							}
							result /= calculation.mod;
							break;
						}
					}
					//вывод результата

					Console.WriteLine (file.Substring(file.LastIndexOf("\\")+1)+" Результат: "+result);
					//проверка на минимум ошибок
					if(bestFileErrors > errors) {
						bestFile = file;
						bestFileErrors = errors;
					}
				}
			}
			//вывод лучшего файла
			Console.WriteLine("Файл с минимальным количеством ошибок: "+bestFile.Substring(bestFile.LastIndexOf("\\")+1)+" (Ошибок: "+bestFileErrors.ToString()+")");
		}
	}
}
