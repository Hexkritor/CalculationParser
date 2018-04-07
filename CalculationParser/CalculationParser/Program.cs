using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace CalculationParser
{

	class MainClass
	{
		public static void Main (string[] args) {
			Stopwatch programTime = new Stopwatch ();
			programTime.Start();
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

					int elementCounter = 0;
					//вычисления результата
					foreach (Calculation calculation in folder.calculations) {
						++elementCounter;
						//проверка на валидность calculation 
						if (!(calculation.hasElement [0] == 1 && calculation.hasElement [1] == 1 && calculation.hasElement [2] == 1 && calculation.hasElement [3] == 0)) {
							errors++;
							Console.WriteLine ("В {0} элементе найдены ошибки:",elementCounter);
							if (calculation.hasElement [0] == 0)
									Console.WriteLine ("Оутствует элемент с названием uid");
							else if (calculation.hasElement [0] > 1)
								Console.WriteLine ("Найдено {0} элементов с названием uid. Требуется уменьшить количество элементов до одного", calculation.hasElement [0]);
							if (calculation.hasElement [1] == 0)
								Console.WriteLine ("Оутствует элемент с названием operand");
							else if (calculation.hasElement [1] > 1)
								Console.WriteLine ("Найдено {0} элементов с названием operand. Требуется уменьшить количество элементов до одного", calculation.hasElement [1]);
							if (calculation.hasElement [2] == 0)
								Console.WriteLine ("Оутствует элемент с названием mod");
							else if (calculation.hasElement [2] > 1)
								Console.WriteLine ("Найдено {0} элементов с названием mod. Требуется уменьшить количество элементов до одного", calculation.hasElement [2]);
							if(calculation.hasElement [3] > 0)
								Console.WriteLine ("Найдено {0} элементов, не относящихся к структуре calculation. Требуется удаление этих элементов", calculation.hasElement [3]);
							continue;
						}
						//сами расчёты
						switch (calculation.operand) {
						case Operand.NONE:
							++errors;
							Console.WriteLine ("В {0} элементе найдены ошибки:",elementCounter);
							Console.WriteLine ("Неверное значение элемента с названием operand. Допустимые значения: add, subtract, multiply, divide");
							break;
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
							//проверка делимости на ноль
							if (calculation.mod == 0) {
								++errors;
								Console.WriteLine ("В {0} элементе найдены ошибки:",elementCounter);
								Console.WriteLine ("Значение элемента с названием mod приводит к делению на ноль. Требуется изменить значение на отличное от нуля");
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
			//вывод лучшего файла и вывод времени
			Console.WriteLine("Файл с минимальным количеством ошибок: "+bestFile.Substring(bestFile.LastIndexOf("\\")+1)+" (Ошибок: "+bestFileErrors.ToString()+")");
			programTime.Stop();
			Console.WriteLine ("Время выполнения: " + programTime.Elapsed.ToString ());
		}
	}
}
