using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cripto_Lab1
{
    class Program
    {
        static class Fisier
        {
            public static string Citire(string file)
            {
                StreamReader sr = new StreamReader(file);
                return sr.ReadToEnd();
            }

            public static void Scriere(string file, string text)
            {
                StreamWriter sw = new StreamWriter(file);
                sw.WriteLine(text);

                sw.Close();
            }
        }





        abstract class Crypt
        {
            public abstract string Encriptare(string s);
            public abstract string Decriptare(string s);
            public abstract string Criptoanaliza(string s);

            protected int cheie;
        }

        class Cezar : Crypt
        {
            public Cezar()
            {
                cheie = 3;
            }

            public override string Encriptare(string s)
            {
                StringBuilder sb = new StringBuilder();
                char aux;

                for (int i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        aux = Convert.ToChar(s[i] + cheie);
                        if (aux > 'Z')
                            aux = Convert.ToChar(aux - 'Z' - 1 + 'A');
                    }
                    else
                        aux = s[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            public override string Decriptare(string s)
            {
                StringBuilder sb = new StringBuilder();
                char aux;

                for (int i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        aux = Convert.ToChar(s[i] - cheie);
                        if (aux < 'A')
                            aux = Convert.ToChar('Z' - 'A' + 1 + aux);
                    }
                    else
                        aux = s[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            public override string Criptoanaliza(string s)
            {
                throw new NotImplementedException();
            }
        }

        class PlusN : Crypt
        {
            public PlusN(int n)
            {
                cheie = n;
            }

            public override string Encriptare(string s)
            {
                StringBuilder sb = new StringBuilder();
                char aux;

                for (int i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        aux = Convert.ToChar(s[i] + cheie);
                        if (aux > 'Z')
                            aux = Convert.ToChar(aux - 'Z' - 1 + 'A');
                    }
                    else
                        aux = s[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            public override string Decriptare(string s)
            {
                return DecriptareEfectiva(s, cheie);
            }

            private string StabilirePrimulCuvant(string s)
            {
                StringBuilder sb = new StringBuilder();
                int i = 0;

                while (s[i] < 'A' || s[i] > 'Z')
                    i++;

                while (s[i] >= 'A' && s[i] <= 'Z')
                {
                    sb.Append(s[i]);
                    i++;
                }

                return sb.ToString();
            }

            private string DecriptareEfectiva(string text, int nr)
            {
                StringBuilder sb = new StringBuilder();
                char aux;

                for (int i = 0; i < text.Length; i++)
                {
                    if ('A' <= text[i] && text[i] <= 'Z')
                    {
                        aux = Convert.ToChar(text[i] - nr);
                        if (aux < 'A')
                            aux = Convert.ToChar('Z' - 'A' + 1 + aux);
                    }
                    else
                        aux = text[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            private void FillList(List<string> words, string file)
            {
                StreamReader sr = new StreamReader(file);
                StringBuilder sb = new StringBuilder();

                while (!sr.EndOfStream)
                {
                    char c = Convert.ToChar(sr.Read());
                    while (c != '\n')
                    {
                        sb.Append(c);
                        c = Convert.ToChar(sr.Read());
                    }

                    words.Add(sb.ToString().ToUpper());
                    sb.Clear();
                }
            }

            private bool Exista(string word, List<string> words)
            {
                for (int i = 0; i < words.Count; i++)
                    if (word == words[i])
                        return true;
                return false;
            }

            public override string Criptoanaliza(string s)
            {
                List<string> words = new List<string>();
                FillList(words, @"wlist_match1.txt");

                string word = StabilirePrimulCuvant(s);

                int i = 0;
                string word2 = DecriptareEfectiva(word, i);

                while (!Exista(word2, words))
                {
                    Console.WriteLine(word2); Console.ReadKey();
                    i++;
                    word2 = DecriptareEfectiva(word, i);
                }

                Console.WriteLine("Cuvant cu sens: " + word2);

                return DecriptareEfectiva(s, i);
            }
        }

        class Monoalfabetica : Crypt
        {
            char[] permutare = new char['Z' - 'A' + 1];
            Random rnd = new Random();

            private struct litere
            {
                public char[] caracter; public float[] aparitie;

                public litere(int n)
                {
                    caracter = new char[n];
                    aparitie = new float[n];

                    char c = 'A';
                    for (int i = 0; i < n; i++)
                    {
                        caracter[i] = c;
                        aparitie[i] = 0;

                        c++;
                    }
                }
            }

            public Monoalfabetica()
            {
                int i = 0;
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    permutare[i] = c;
                    i++;
                }
            }

            public override string Encriptare(string s)
            {
                int i, j; char aux;
                for (i = 0; i <= permutare.Length - 2; i++)
                {
                    j = rnd.Next(i, permutare.Length);

                    aux = permutare[i];
                    permutare[i] = permutare[j];
                    permutare[j] = aux;
                }

                StringBuilder sb = new StringBuilder();
                for (i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                        aux = permutare[s[i] - 'A'];
                    else
                        aux = s[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            public override string Decriptare(string s)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        for (int j = 0; j < permutare.Length; j++)
                            if (permutare[j] == s[i])
                                sb.Append(Convert.ToChar('A' + j));
                    }
                    else
                        sb.Append(s[i]);
                }

                return sb.ToString();
            }

            public override string Criptoanaliza(string s)
            {
                litere L1 = new litere('Z' - 'A' + 1); // aparitia literelor intr-un text oarecare
                litere L2 = new litere('Z' - 'A' + 1); // aparitia literelor in textul criptat

                EnumerareLitere(ref L1, @"Random_text.txt");
                EnumerareLitere(ref L2, @"text_criptat.txt");

                SortareDesc(ref L1);
                SortareDesc(ref L2);

                StringBuilder sb = new StringBuilder();
                int j;

                for (int i = 0; i < s.Length; i++)
                {
                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        // cautam litera potrivita in lista de aparitie
                        for (j = 0; s[i] != L2.caracter[j]; j++) ;

                        // adaugam litera corespunzatoara
                        sb.Append(L1.caracter[j]);
                    }
                    else
                        sb.Append(s[i]);
                }

                return sb.ToString();
            }

            private void SortareDesc(ref litere L)
            {
                bool ok = false;
                int n = L.caracter.Length;

                do
                {
                    ok = true;
                    for (int i = 0; i < n - 1; i++)
                        if (L.aparitie[i] < L.aparitie[i + 1])
                        {
                            ok = false;
                            Schimba<float>(ref L.aparitie[i], ref L.aparitie[i + 1]);
                            Schimba<char>(ref L.caracter[i], ref L.caracter[i + 1]);
                        }
                } while (!ok);


            }

            private void Schimba<T>(ref T a, ref T b)
            {
                T p = a;
                a = b;
                b = p;
            }

            private void EnumerareLitere(ref litere L, string file)
            {
                L = new litere('Z' - 'A' + 1);
                StreamReader sr = new StreamReader(file);

                char c; int i;

                do
                {
                    c = Convert.ToChar(sr.Read());
                    c = char.ToUpper(c);

                    if ('A' <= c && c <= 'Z')
                    {
                        i = 0;
                        while (c != L.caracter[i]) i++;

                        L.aparitie[i]++;
                    }

                } while (!sr.EndOfStream);

            }
        }

        class Vigenere : Crypt
        {
            char[,] permutari;
            Random rnd = new Random();

            public Vigenere(int n)
            {
                cheie = n;
                permutari = new char[cheie, 'Z' - 'A' + 1];

                Init(permutari, cheie);
            }

            private void Init(char[,] permutari, int cheie)
            {
                for (int i = 0; i < cheie; i++)
                {
                    int j = 0;
                    for (char c = 'A'; c <= 'Z'; c++)
                    {
                        permutari[i, j] = c;
                        j++;
                    }
                }
            }

            public override string Encriptare(string s)
            {
                int i, j = 0; char aux;

                for (i = 0; i < cheie; i++)
                    FisherYates(permutari, i);

                StringBuilder sb = new StringBuilder();

                for (i = 0; i < s.Length; i++)
                {
                    if (j == cheie)
                        j = 0;

                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        aux = permutari[j, s[i] - 'A'];
                        j++;
                    }
                    else
                        aux = s[i];

                    sb.Append(aux);
                }

                return sb.ToString();
            }

            private void FisherYates(char[,] permutari, int nr)
            {
                int i, j; char aux;
                for (i = 0; i <= permutari.GetLength(1) - 2; i++)
                {
                    j = rnd.Next(i, permutari.GetLength(1));

                    aux = permutari[nr, i];
                    permutari[nr, i] = permutari[nr, j];
                    permutari[nr, j] = aux;
                }
            }

            public override string Decriptare(string s)
            {
                StringBuilder sb = new StringBuilder();

                int j = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (j == cheie)
                        j = 0;

                    if ('A' <= s[i] && s[i] <= 'Z')
                    {
                        for (int k = 0; k < permutari.GetLength(1); k++)
                            if (permutari[j, k] == s[i])
                                sb.Append(Convert.ToChar('A' + k));
                        j++;
                    }
                    else
                        sb.Append(s[i]);
                }

                return sb.ToString();
            }

            public override string Criptoanaliza(string s)
            {
                throw new NotImplementedException();
            }
        }

        static void Main(string[] args)
        {
            int alg, opt, n = 0;
            Crypt C = null;

            // meniu pentru alegerea algoritmului de criptare
            do
            {
                Console.WriteLine("Alegeti algoritmul: ");
                Console.WriteLine("1. Cezar ");
                Console.WriteLine("2. PlusN ");
                Console.WriteLine("3. Monoalfabetica ");
                Console.WriteLine("4. Polialfabetica ");

                alg = Convert.ToInt32(Console.ReadLine());

                if (alg == 2 || alg == 4)
                {
                    Console.WriteLine("Introduceti numarul cheie: ");
                    n = Convert.ToInt32(Console.ReadLine());
                }

                switch (alg)
                {
                    case 1: C = new Cezar(); break;
                    case 2: C = new PlusN(n); break;
                    case 3: C = new Monoalfabetica(); break;
                    case 4: C = new Vigenere(n); break;
                }

            } while (alg <= 0 || alg > 4);


            // meniu pentru lucrarea cu algoritmul alesa
            Console.WriteLine("Alegeti optiunea: ");
            Console.WriteLine("1. Encriptare ");
            Console.WriteLine("2. Decriptare ");
            Console.WriteLine("3. Criptoanaliza ");

            opt = Convert.ToInt32(Console.ReadLine());

            string text = null, filepath = @"";

            if (opt == 1)
                text = Fisier.Citire(filepath + "text_clar.txt");
            else if (opt == 2 || opt == 3)
                text = Fisier.Citire(filepath + "text_criptat.txt");

            switch (opt)
            {
                case 1: text = C.Encriptare(text); break;
                case 2: text = C.Decriptare(text); break;
                case 3: text = C.Criptoanaliza(text); break;
                default: Console.WriteLine("Optiune introdusa gresita!"); break;
            }

            if (opt == 1)
                Fisier.Scriere(filepath + "text_criptat.txt", text);
            else if (opt == 2 || opt == 3)
                Fisier.Scriere(filepath + "text_clar.txt", text);
        }
    }
}
