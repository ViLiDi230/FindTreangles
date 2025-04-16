using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Second
{
    class Program
    {
		public List<Sides> Storoni = new List<Sides>();
        public List<double> Cvadrati = new List<double>();
        public List<double> StoroniLength = new List<double>();
		public List<Cor> Coordinates = new List<Cor>();
		public List<Cor> Coordinatescheck = new List<Cor>();
		public List<Triangle> TrianglesList = new List<Triangle>();
        public Dictionary<double, List<Triangle>> dicTriangles = new Dictionary<double, List<Triangle>>();

		static void Main(string[] args)
        {
			var Tochka = new List<Cor>();
			bool glush = false;
			var startX = 0; // Смещение по X
			var startY = 0; // Начальная координата Y
			var program = new Program();

			Console.WriteLine("Введите кол-во квадратов");
			int count = int.Parse(Console.ReadLine());

			for (int i = 0; i < count; i++)
			{
				Console.WriteLine("Введите сторону квадрата");
				int h = int.Parse(Console.ReadLine());

				// Координаты квадратов
				Cor A = new Cor(startX, startY);
				Cor B = new Cor(startX, startY + h);
				Cor C = new Cor(startX + (double)h / 2, startY + (double)h / 2);
				Cor D = new Cor(startX + h, startY);
				Cor E = new Cor(startX + h, startY + h);

				Tochka = new List<Cor> { A, B, D, E, C};
				program.Cvadrati.Add(h);
				if (i > 0)
				{
					Cor Border = new Cor(startX, startY + (double)h / 2);
					Tochka.Add(Border);
				}

				for (int j = 0; j < Tochka.Count(); j++)
				{
					if (!program.Coordinates.Contains(Tochka[j]))
					{
						program.Coordinates.Add(Tochka[j]);
					}
				}

				// Перемещение вправо, а не вверх
				startX += h;
			}
			//program.Coordinatescheck = new List<Cor> { A, B, E };
			program.FindStoroni(program.Coordinates);
            program.FindTreug(program.Storoni);
            program.GetDicTrig(program.TrianglesList);
			program.Vivod(program.Cvadrati);
			Process.Start(new ProcessStartInfo //открываем результат
			{
				FileName = "1.txt",
				UseShellExecute = true //Система выбирает программу
			});
		}

        private void Vivod(List<double> h)
        {
            using (StreamWriter sw = new StreamWriter("1.txt"))
            {
                sw.WriteLine($"Количество квадратов = {Cvadrati.Count()}");
                sw.WriteLine($"Длины сторон квадратов:");
                foreach (var item in Cvadrati)
                {
                    sw.WriteLine($"{item.ToString()}");
                }
                sw.WriteLine($"Точки:({Coordinates.Count()})");
                foreach (var item in Coordinates)
                {
                    sw.WriteLine($"{item.ToString()}");
                }
                sw.WriteLine($"Стороны: ({Storoni.Count()})");
                foreach (var item in Storoni)
                {
                    sw.WriteLine($"{item.ToString()}");
                }
                sw.WriteLine("Длины сторон:");
                foreach (var item in StoroniLength)
                {
                    sw.WriteLine($"{item.ToString()}");
                }
                sw.WriteLine($"Треугольники:({TrianglesList.Count()})");
                foreach (double key in dicTriangles.Keys)
                {
                    sw.WriteLine($"{dicTriangles[key].Count} треугольников с площадью: {key} с координатами:");
                    foreach (Triangle triangle in dicTriangles[key])
                    {
                        sw.WriteLine($"[{triangle.ToString()}]");
                    }

                }
				//sw.WriteLine($"{item.ToString()}");
            }
        }
        public void FindStoroni(List<Cor> Точки)
        {
            for (int i =0; i< Точки.Count(); i++)
            {
                for (int j = 0; j< Точки.Count();j++)
                {
                    var side = new Sides(Точки[i], Точки[j]);
                    if(side.first != side.second && !Storoni.Contains(side) && !Storoni.Contains(side.Reverse()))
                    {
                        StoroniLength.Add(side.Length());
                        Storoni.Add(side);
                    }
                }
            }
        }
        private List<Triangle> FindTreug(List<Sides>sides)
        {
            try
            {
                for (int i = 0; i< sides.Count; i++)
                {
                    for (int j = 0; j < sides.Count; j++)
                    {
                        for (int k = 0; k< sides.Count; k++)
                        {
                            if (sides[i] != sides[j] && sides[i] != sides[k] && sides[j] != sides[k] && sides[j].TrySide(sides[i].second) && sides[k].TrySide(sides[j].second, sides[i].first))
                            {
                                if (IsTriangleExist(sides[i].Length(),sides[j].Length(),sides[k].Length()))
                                {
                                    var temp = new Triangle(sides[i], sides[j], sides[k]);
                                    if (!TrianglesList.Contains(temp))
                                    {
										TrianglesList.Add(temp);
									}
								}
                            }
                        }
                    }
                }
				return TrianglesList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
		public Dictionary<double, List<Triangle>> GetDicTrig(List<Triangle> triangles)
		{
			
			foreach (Triangle triangle in triangles)
			{
				double Square = triangle.GetSquare();
				if (dicTriangles.TryGetValue(Square, out List<Triangle> value))
				{
					value.Add(triangle);
				}
				else
				{
					var temp = new List<Triangle>();
					temp.Add(triangle);
					dicTriangles.Add(Square, temp);
				}
			}
			return dicTriangles;
		}
		private bool IsTriangleExist(double a,double b, double c)
        {
			if (a <= 0 || b <= 0 || c <= 0)
				return false;
			return (a + b > c) && (a + c > b) && (b + c > a);
        }
    }
    class Triangle
    {
		public List<Sides> Storoni;
		public Sides first { get; set; }
        public Sides second { get; set; }
        public Sides third { get; set; }
        public Triangle(Sides First, Sides Second, Sides Third)
        {
            first = First; second = Second; third = Third;
            Storoni = new List<Sides> { first, second, third };
        }
		public List<Cor> GetCoordinate()
		{
			List<Cor> kors = new List<Cor>();
			foreach (Sides side in Storoni)
			{
				foreach (Cor kor in side.Coordinates)
				{
					if (!kors.Contains(kor))
					{
						kors.Add(kor);
						if (kors.Count == 3) return kors;
					}
				}
			}
			return kors;
		}
		public double GetSquare()
		{
			List<Cor> kors = GetCoordinate();
			return 0.5 * Math.Abs(kors[0].first * (kors[1].second - kors[2].second) + kors[1].first * (kors[2].second - kors[0].second) + kors[2].first * (kors[0].second - kors[1].second));
		}
		public static bool operator ==(Triangle one, Triangle two)
		{
			if (ReferenceEquals(one, two)) return true;
			if (one is null || two is null) return false;
			return one.Sort().SequenceEqual(two.Sort());
			//return one.first == two.first && one.second == two.second && one.third == two.third;
		}
		public static bool operator !=(Triangle one, Triangle two)
		{
			return !(one == two);
		}
		public List<Sides> Sort()
        {
			var sidesList = new List<Sides> { first, second, third };
			sidesList.Sort((a, b) => a.ToString().CompareTo(b.ToString())); // Сортируем по строковому представлению
			return sidesList;
		}
		public override string ToString()
        {
            return $"({first};{second};{third})";
        }
		public override bool Equals(object obj)
		{
			return obj is Triangle other && this == other;
		}

		public override int GetHashCode()
		{
			return first.GetHashCode() ^ second.GetHashCode();
		}
	}
    class Sides
    {
        public List<Cor> Coordinates;
		public Cor first { get; set; }
        public Cor second { get; set; }
        public Sides(Cor First, Cor Second)
        {
            first = First;
            second = Second;
            Coordinates = new List <Cor> {First,Second};
        }
		public bool TrySide(Cor startPoint)
		{
			foreach (Cor point in Coordinates)
			{
				if (point == startPoint)
				{
					return true;
				}
			}
			return false;
		}
		public bool TrySide(Cor startPoint, Cor endPoint)
		{
			if ((first == startPoint && second == endPoint) || (second == startPoint && first == endPoint)) return true;
			else return false;
		}
		public Sides Reverse()
        {
            return new Sides(second, first);
        }
        public double Length()
        {
            return Math.Sqrt((Math.Pow(first.first - second.first, 2)) + (Math.Pow(first.second - second.second, 2)));
        }
        public static bool operator >(Sides one, Sides two)
        {
            if (one.first > two.first)
                return true;
            else if (one.first == two.first)
                return one.second > two.second;
            return false;
        }
        public static bool operator <(Sides one, Sides two)
        {
            if (one.first < two.first)
                return true;
            else if (one.first == two.first)
                return one.second < two.second;
            return false;
        }
        public static bool operator ==(Sides one, Sides two)
        {
            if (one.first == two.first)
            {
                return one.second == two.second;
            }
            return false;
        }
        public static bool operator !=(Sides one, Sides two)
        {
            if(one.first == two.first)
            {
                return one.second != two.second;
            }
            return true;
        }
		public override bool Equals(object obj)
		{
			return obj is Sides other && this == other;
		}
		public override int GetHashCode()
		{
			return first.GetHashCode() ^ second.GetHashCode();
		}
		public override string ToString()
        {
            return $"({first.ToString()},{second.ToString()})";
        }
    }
    class Cor
    {
        public double first { get; set; }
        public double second { get; set; }
        public Cor(double First, double Second)
        {
            first = First;
            second = Second;
        }
        public static Cor operator +(Cor one, Cor two)
        {
            return new Cor ((one.first + two.first),(one.second + two.second)); 
        }
        public static Cor operator -(Cor one, Cor two)
        {
            return new Cor((one.first - two.first), (one.second - two.second));
        }
        public static bool operator >(Cor one, Cor two)
        {
            if (one.first > two.first)
                return true;
            else if (one.first == two.first)
                return one.second > two.second;
            return false;
        }
        public static bool operator <(Cor one, Cor two)
        {
            if (one.first < two.first)
                return true;
            else if (one.first == two.first)
                return one.second < two.second;
            return false;
        }
        public static bool operator ==(Cor one, Cor two)
        {
            return one.first == two.first && one.second == two.second;
        }
        public static bool operator !=(Cor one, Cor two)
        {
            return one.first != two.first || one.second != two.second;
		}
		public override bool Equals(object obj)
		{
			return obj is Cor other && this == other;
		}

		public override int GetHashCode()
		{
			return first.GetHashCode() ^ second.GetHashCode();
		}
		public override string ToString()
        {
            return $"({first};{second})";
        }
    }
}
