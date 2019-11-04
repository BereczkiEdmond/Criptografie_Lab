using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cripto_Lab2
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
        }

        class Jefferson : Crypt
        {
            private class Disc
            {
                public char[] CharSet { get; private set; }  // caractere de pe disc
                public int ID { get; private set; } // numar unic pentru identificare

                public Disc(string s, int id)
                {
                    CharSet = new char[s.Length];
                    for (int i = 0; i < s.Length; i++)
                        CharSet[i] = s[i];
                    ID = id;
                }

                public char Rotate(int start, int offset)
                {
                    int aux = (start + offset) % CharSet.Length;
                    if (aux < 0)
                        aux = CharSet.Length + aux;
                    return CharSet[aux];
                }
            }

            private int n;  // numarul de discuri
            private Random rnd;
            private int[] key;  // ordinea discurilor numerotate
            private Disc[] D;
            private int offset;

            public Jefferson(int val)
            {
                n = val;
                key = new int[n];
                rnd = new Random();

                // Crearea alfabetului cu litere mari
                StringBuilder sb = new StringBuilder();
                for (char c = 'A'; c <= 'Z'; c++)
                    sb.Append(c);
                string alfabet = sb.ToString();

                // Crearea discurilor si amestecarea caracterelor
                D = new Disc[n];
                for (int i = 0; i < n; i++)
                {
                    D[i] = new Disc(alfabet, i);
                    FisherYates(D[i].CharSet);
                    key[i] = i;
                }  
            }

            public override string Encriptare(string s)
            {
                FisherYates(key);   // generarea ordinii aleatorie a discurilor
                
                StringBuilder sb = new StringBuilder();
                offset = rnd.Next(1, n);    // adaugam o pozitie aleatoriu la textul clar
                                            // pentru a obtine textul criptat
                int j, k = 0; char c;
                for (int i = 0; i < s.Length; i++)
                {
                    // cautam pozitia caracterului pe disc
                    j = 0;
                    while (s[i] != D[key[k]].CharSet[j])
                        j++;

                    // rotim discul ca sa obtinem caracterul criptat
                    c = D[key[k]].Rotate(j, offset);

                    // mergem mai departe
                    sb.Append(c);
                    k++;

                    // cand nu mai avem discuri incepem criptarea cu primul disc
                    // pana cand nu se termina textul pe care trebuie criptat
                    if (k == n)
                        k = 0;
                }

                return sb.ToString();
            }

            public override string Decriptare(string s)
            {
                StringBuilder sb = new StringBuilder();
                int j, k = 0; char c;
                for (int i = 0; i < s.Length; i++)
                {
                    // cautam pozitia caracterului criptat pe disc
                    j = 0;
                    while (s[i] != D[key[k]].CharSet[j])
                        j++;

                    // rotim discul ca sa obtinem caracterul clar
                    c = D[key[k]].Rotate(j, -offset);

                    sb.Append(c);
                    k++;

                    if (k == n)
                        k = 0;
                }

                return sb.ToString();
            }

            public override string Criptoanaliza(string s)
            {
                throw new NotImplementedException();
            }


            // Functii ajutatoare
            private void FisherYates<T>(T[] vector)
            {
                int i, j; T aux;
                for (i = 0; i <= vector.Length - 2; i++)
                {
                    j = rnd.Next(i, vector.Length);

                    aux = vector[i];
                    vector[i] = vector[j];
                    vector[j] = aux;
                }
            }
        }

        class Playfair : Crypt
        {
            private char[,] key;

            private struct Pos {
                public int row, col;
                public Pos(int r, int c)
                { row = r; col = c; }
            };

            public Playfair(string keytext)
            {
                // constructorul contine crearea matricii cheie
                key = new char[5, 5];
                bool[] FreeChars = new bool[26];
                int i, j, k;

                for (i = 0; i < FreeChars.Length; i++)
                    FreeChars[i] = true;
                FreeChars['J' - 'A'] = false; // litera J este caracterul omisa

                i = j = 0;
                while (i < 25 && j < keytext.Length)
                {
                    if (FreeChars[keytext[j] -'A']) // daca litera din keytext este libera
                    {
                        key[i / 5, i % 5] = keytext[j];
                        FreeChars[keytext[j] - 'A'] = false;

                        i++;
                    }

                    j++;
                }

                // daca nu mai sunt litere din keytext si au ramas
                // spatii libere completam in ordinea alfabetica
                k = 0;
                while (i < 25)
                {
                    while (!FreeChars[k])
                        k++;
                    key[i / 5, i % 5] = (char)('A' + k);
                    FreeChars[k] = false;

                    i++;
                }
            }

            public override string Encriptare(string s)
            {
                string[] part = Split(s, 2, 'X');

                for (int i = 0; i < part.Length; i++)
                {
                    Pos[] p;
                    GetPos(part[i][0], part[i][1], out p);
                    part[i] = Change(p, RelativePos(p[0], p[1]));
                }

                return Merge(part);
            }

            public override string Decriptare(string s)
            {
                string[] part = Split(s, 2);

                for (int i = 0; i < part.Length; i++)
                {
                    Pos[] p;
                    GetPos(part[i][0], part[i][1], out p);
                    part[i] = Change(p, RelativePos(p[0], p[1]), true);
                }

                return Merge(part);
            }

            public override string Criptoanaliza(string s)
            {
                throw new NotImplementedException();
            }

            // Functii ajutatoare
            private void GetPos(char a, char b, out Pos[] C)
            {
                C = new Pos[2];
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                    {
                        if (key[i, j] == a)
                            C[0] = new Pos(i, j);
                        else if (key[i, j] == b)
                            C[1] = new Pos(i, j);
                    }
            }

            private int RelativePos(Pos a, Pos b)
            {
                if (a.row == b.row)
                    return 1;   // acelasi linie
                else if (a.col == b.col)
                    return 2;   // acelasi coloana
                else
                    return 3;   // niciunul
            }

            private string[] Split(string s, int n, char empty = ' ')
            {
                // functie pentru despartirii stringul "s" in partile cu "n" lungime
                // completand spatiile ramase la capat si intre caractere duble 
                // cu caractere "empty" daca este specificat

                List<char> L = s.ToList();

                if (empty != ' ')
                    for (int i = 1; i < L.Count; i++)
                        if (L[i] == L[i - 1])
                            L.Insert(i, empty);

                while (L.Count % n != 0)
                    L.Add(empty);

                int val = L.Count / n; // "val" este numarul substringurilor
                string[] part = new string[val];
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < val; i++)
                {
                    for (int j = 0; j < n; j++)
                        sb.Append(L[i * n + j]);

                    part[i] = sb.ToString();
                    sb.Clear();
                }

                return part;
            }

            private string Change(Pos[] part, int option, bool reverse = false)
            {
                int val = 1;
                char a, b; Pos ap, bp;

                if (reverse)
                {
                    val = -1;

                    for (int i = 0; i <= 1; i++)
                    {
                        if (part[i].col == 0 && option == 1)
                            part[i].col = 5;
                        if (part[i].row == 0 && option == 2)
                            part[i].row = 5;
                    }
                }

                switch(option)
                {
                    case 1:
                        {
                            ap.col = (part[0].col + val) % 5;
                            ap.row = part[0].row;

                            bp.col = (part[1].col + val) % 5;
                            bp.row = part[1].row;
                        }
                        break;
                    case 2:
                        {
                            ap.col = part[0].col;
                            ap.row = (part[0].row + val) % 5;

                            bp.col = part[1].col;
                            bp.row = (part[1].row + val) % 5;
                        }
                        break;
                    case 3:
                        {
                            ap.row = part[0].row;
                            ap.col = part[1].col;

                            bp.row = part[1].row;
                            bp.col = part[0].col;
                        }
                        break;
                    default: throw new Exception("Functie Change(Pos[] part, int option): variabila option are valoare gresita");
                }

                a = key[ap.row, ap.col];
                b = key[bp.row, bp.col];

                StringBuilder sb = new StringBuilder();
                sb.Append(a); sb.Append(b);

                return sb.ToString();
            }

            private string Merge(string[] part)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < part.Length; i++)
                    for (int j = 0; j < part[i].Length; j++)
                        sb.Append(part[i][j]);

                return sb.ToString();
            }
        }

        // functie pentru eliminarea caracterelor nedorite dintr-un text
        public static string RemoveFromText(string text, string chrs)
        {
            StringBuilder sb = new StringBuilder();
            bool ok;
            for (int i = 0; i < text.Length; i++)
            {
                ok = true;
                for (int j = 0; j < chrs.Length; j++)
                    if (text[i] == chrs[j])
                        ok = false;

                if (ok)
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            Crypt C; int opt;

            string text = Fisier.Citire("text_clar.txt");
            text = text.ToUpper();
            text = RemoveFromText(text, " ,.");

            Console.WriteLine("Alegeti optiunea dorita: ");
            Console.WriteLine("1. Jefferson ");
            Console.WriteLine("2. Playfair ");
            opt = int.Parse(Console.ReadLine());

            switch(opt)
            {
                case 1:
                    {
                        Console.Write("Introduceti numarul discurilor: ");
                        int n = int.Parse(Console.ReadLine());
                        C = new Jefferson(n);
                    }
                    break;
                case 2:
                    {
                        Console.Write("Introduceti textul cheie: ");
                        string k = Convert.ToString(Console.ReadLine());
                        k = k.ToUpper();
                        C = new Playfair(k);
                    }
                    break;
                default: throw new Exception("Optiune introdusa este invalida!");
            }

            Console.WriteLine("Textul clar: " + text);
            text = C.Encriptare(text);
            Console.WriteLine("Textul dupa criptare: " + text);
            text = C.Decriptare(text);
            Console.WriteLine("Textul dupa decriptare: " + text);
        }
    }
}
