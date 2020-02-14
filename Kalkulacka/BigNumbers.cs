using System;
using System.Collections.Generic;
using System.Text;

namespace Kalkulacka
{

    internal class BigNumbers
    {

        // List do ktereho se ukladaji vsechna cisla z vyrazu a vysledna cisla vracena metodami operaci
        private List<int[]> list;
        public List<int[]> List { get => list; private set => list = value; }

        private List<int> numberOfNines = new List<int>(10);

        private List<int> digits = new List<int>(10);

        // Slouzi k vypocitavani vysledku nasobeni; ukladaji se do neho prubezne vysledky nasobeni
        private List<int[]> nasobeni = new List<int[]> { new int[1], new int[1] };

        // Pracuje spolecne s listem nasobeni, urcuje pocet cislic jednotlivych cisel
        private int[] nasobeniDigits = new int[2];

        // Urcuje pocet sad cisel v listu na korespondujicich mistech se stejnym indexem
        public List<int> NumberOfNines { get => numberOfNines; set => numberOfNines = value; }

        // Urcuje pocet cislic cisel v listu na korespondujicich mistech se stejnym indexem
        public List<int> Listdigits { get => digits; set => digits = value; }

        // Slouzi k vypocitavani vysledku deleni; ukladaji se do neho prubezne vysledky deleni
        private List<int[]> deleni = new List<int[]> { new int[1], new int[1], new int[1], new int[1] };

        // Pracuje spolecne s listem deleni, urcuje pocet cislic jednotlivych cisel
        private int[] deleniDigits = new int[2];

        /// <summary>
        /// Analyzuje vstup, zjisti kolik mista vynahradi v listu (neboli kolik je ve vyrazu cisel) a u kazdeho cisla ulozi 
        /// pocet cislic do ListDigits
        /// </summary>
        /// <param name="textBox"></param>
        public void AnalyzeInput(string textBox)
        {
            int NumberOfDigits = 0;
            int i = 0;
            int pocetCisel = 0;
            while (i < textBox.Length)
            {
                if (textBox[i] > 47 && textBox[i] < 58)
                {
                    while (i < textBox.Length && textBox[i] > 47 && textBox[i] < 58)
                    {
                        NumberOfDigits++;
                        i++;
                    }

                    if (i < textBox.Length && !((textBox[i] > 39 && textBox[i] < 44) || textBox[i] == 45 || textBox[i] == 47 || textBox[i] == ' '))
                    {
                        Console.WriteLine("Zadany vyraz obsahuje nepovoleny znak \"{0}\" na {1}. miste. Uprav vyraz do spravneho tvaru, ktery obshauje pouze povolene operace a cislice.", textBox[i], i);
                        return;
                    }
                    Listdigits.Add(NumberOfDigits);
                    numberOfNines.Add(DigitsToNines(NumberOfDigits));
                    NumberOfDigits = 0;
                    pocetCisel++;
                }
                else if (!((textBox[i] > 39 && textBox[i] < 44) || textBox[i] == 45 || textBox[i] == 47))
                {
                    Console.WriteLine("Zadany vyraz obsahuje nepovoleny znak. Uprav vyraz do spravneho tvaru, ktery obshauje pouze povolene oprace a cislice.");
                    return;
                }
                i++;
                list = new List<int[]>(pocetCisel);
            }
            return;
        }


        /// <summary>
        /// Nacte vstup uzivatele a vrati Tuple s prepisem v poli stringu a pomocne bool pole s hodnotou TRUE na korespondujicich mistech operatoru
        /// </summary>
        /// <returns></returns>
        public Tuple<string[], bool[]> ReadInput(string textBox)
        {
            int i = 0;
            string b = "";
            string[] output = new string[100];
            bool[] op = new bool[100];
            int nines = 0;
            int outputIndex = 0;
            int numbersIndex = 0;
            int maxNumber = Listdigits[0];
            string tmp = "";

            foreach (int digit in Listdigits)
            {
                if (digit > maxNumber)
                    maxNumber = digit;
            }

            int[] number = new int[maxNumber];

            while (i < textBox.Length)
            {
                if (textBox[i] > 47 && textBox[i] < 58)
                {

                    while (i != textBox.Length && textBox[i] > 47 && textBox[i] < 58)
                    {
                        b = b + char.ToString(textBox[i]);
                        if (b.Length == 9 || i + 1 == textBox.Length || textBox[i + 1] <= 47 || textBox[i + 1] >= 58)
                        {
                            if (b[0] == '0')
                            {
                                while (tmp.Length != 10 - b.Length)
                                {
                                    tmp += "1";
                                }
                                b = tmp + b;
                                tmp = "";
                            }
                            number[nines] = int.Parse(b);
                            b = "";
                            if (i + 1 == textBox.Length || textBox[i + 1] <= 47 || textBox[i + 1] >= 58)
                            {
                                int[] number_tmp = new int[nines + 1];
                                for (int tmp1 = 0, tmp2 = nines; tmp1 < nines + 1; ++tmp1, --tmp2)
                                    number_tmp[tmp1] = number[tmp2];
                                list.Add(number_tmp);
                                NumberOfNines[numbersIndex] = nines + 1;
                                nines = 0;
                                output[outputIndex] = numbersIndex.ToString();
                                outputIndex++; numbersIndex++;

                            }
                            else
                            {
                                nines++;
                            }
                        }
                        i++;
                    }
                }
                else
                {
                    if (textBox[i] != ' ')
                    {
                        if ((textBox[i] > 39 && textBox[i] < 44) || textBox[i] == 45 || textBox[i] == 47)
                        {
                            output[outputIndex] = Char.ToString(textBox[i]);
                            op[outputIndex] = true;
                            outputIndex++;
                        }
                    }
                    i++;
                }
            }
            return Tuple.Create(output, op);
        }

        #region other

        /// <summary>
        /// Prevede infix zapis na postfix, vraci Tuple se Stack<string> obsahujici postfix zapis a pomocne bool pole s hodnotou TRUE na korespondujicich mistech operatoru
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public Tuple<Stack<string>, bool[]> InfixToPostfix(Tuple<string[], bool[]> tuple)
        {
            Stack<string> output = new Stack<string>();
            Stack<string> stackop = new Stack<string>();

            string[] infix = tuple.Item1;
            bool[] op = tuple.Item2;
            bool[] opnew = new bool[100];
            int i = 0;
            int j = 0;
            while (infix[i] != null)
            {
                if (!op[i])
                {
                    output.Push(infix[i]);
                    opnew[j] = false;
                    j++;
                }
                else if ((stackop.Count == 0 || Prednost(infix[i]) > Prednost(stackop.Peek()) || stackop.Peek() == "(") && infix[i] != ")")
                {
                    stackop.Push(infix[i]);
                }
                else
                {
                    while (stackop.Count != 0 && (Prednost(stackop.Peek()) >= Prednost(infix[i])) && stackop.Peek() != "(" && stackop.Peek() != ")")
                    {
                        output.Push(stackop.Pop());
                        opnew[j] = true;
                        j++;
                    }
                    if (infix[i] != ")")
                    { stackop.Push(infix[i]); }
                }
                if (infix[i] == ")")
                {
                    while (stackop.Peek() != "(")
                    {
                        output.Push(stackop.Pop());
                    }
                    stackop.Pop();
                }
                i++;
            }
            while (stackop.Count > 0)
            {
                output.Push(stackop.Pop());
                opnew[j] = true;
                j++;
            }
            return Tuple.Create(output, opnew);
        }
        /// <summary>
        /// Vrati hodnotu prednosti
        /// </summary>
        /// <param name="value"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private int Prednost(string value)
        {
            int hodnota;
            switch (value)
            {
                case "+":
                    hodnota = 0;
                    break;
                case "-":
                    hodnota = 0;
                    break;
                case "*":
                    hodnota = 1;
                    break;
                case "/":
                    hodnota = 1;
                    break;
                case "(":
                    hodnota = 2;
                    break;
                default:
                    hodnota = -1;
                    break;

            }
            return hodnota;
        }
        /// <summary>
        /// Dostane Tuple s infix zapisem v Stack<string> a pomocnym bool polem a vrati vysledek vyrazu <see cref="double"/>
        /// </summary>
        public int VypocetPostfix(Tuple<Stack<string>, bool[]> tuple)
        {
            Stack<string> postfix = tuple.Item1;
            bool[] op = tuple.Item2;
            int i = 0;
            Stack<string> postfix2 = new Stack<string>();
            while (postfix.Count > 0)
            {
                postfix2.Push(postfix.Pop());
            }
            string value;
            int hodnota;
            Stack<int> zasobnik = new Stack<int>();

            while (postfix2.Count > 0)
            {
                value = postfix2.Pop();
                if (!op[i])
                {
                    hodnota = int.Parse(value);
                    zasobnik.Push(hodnota);
                }
                else
                {
                    switch (value)
                    {
                        case "+":
                            zasobnik.Push(Secti(zasobnik.Pop(), zasobnik.Pop(), 0));
                            break;
                        case "-":
                            zasobnik.Push(Substract(zasobnik.Pop(), zasobnik.Pop(), 0));
                            break;
                        case "*":
                            zasobnik.Push(Nasob(zasobnik.Pop(), zasobnik.Pop()));
                            break;
                        case "/":
                            zasobnik.Push(Divide(zasobnik.Pop(), zasobnik.Pop(), 5));
                            break;
                    }
                }
                i++;
            }
            return zasobnik.Pop();
        }

        #endregion

        /// <summary>
        /// Secte cisla z listu na pozicich number1 a number2
        /// </summary>
        /// <param name="number1"></param>
        /// <param name="number2"></param>
        /// <param name="vstupniRetezec">Pokud je 0, cisla se nacitaji z obecneho listu, pokud je vetsi, cisla se nacitaji z listu nasobeni</param>
        /// <returns>Vrati pozici souctu v listu</returns>
        private int Secti(int number1, int number2, int vstupniRetezec)
        {
            int coupleOne = 0; //Index sad prvniho cisla
            int coupleTwo = 0; //Index sad druheho cisla
            int[] numberOne; 
            int[] numberTwo;
            int lengthNewNumber;
            int[] newNumber;
            int length1; //Pocet sad prvniho cisla
            int length2; //Pocet sad druheho cisla
            int digitsNasobitel = 0; 

            int remove = 0;
            string tmp = "";

            if (vstupniRetezec > 0)
                digitsNasobitel = vstupniRetezec - 1; //Urcuje pocet nul druheho cisla

            //Nacita cisla z obecneho listu
            if (vstupniRetezec == 0)
            {
                numberOne = list[number1];
                numberTwo = list[number2];
                lengthNewNumber = Math.Max(NumberOfNines[number1] + 1, NumberOfNines[number2] + 1);
                newNumber = new int[lengthNewNumber];
                length1 = NumberOfNines[number1];
                length2 = NumberOfNines[number2];
            }
            //Nacita cisla z listu nasobeni
            else
            {
                numberOne = nasobeni[0];
                numberTwo = nasobeni[1];
                lengthNewNumber = DigitsToNines(Math.Max(nasobeniDigits[0] + 1, nasobeniDigits[1] + 1 + vstupniRetezec));
                newNumber = new int[lengthNewNumber];
                length1 = DigitsToNines(nasobeniDigits[0]);
                length2 = DigitsToNines(nasobeniDigits[1]);
            }

            string currentOne = numberOne[0].ToString();
            //Upravuje prvni sadu pro nuly na zacatku
            if (currentOne.Length > 9)
            {
                while (currentOne[remove] == '1')
                {
                    remove++;
                }
                currentOne = currentOne.Remove(0, remove);
                remove = 0;
            }

            string currentTwo = numberTwo[0].ToString();
            //Upravuje sadu druheho cislo pro nuly na zacatku
            if (currentTwo.Length > 9)
            {
                while (currentTwo[remove] == '1')
                {
                    remove++;
                }
                currentTwo = currentTwo.Remove(0, remove);
                remove = 0;
            }

            string number = ""; //Do number se ukladaji jednotlive sady ve forme stringu
            int i = 1, j = 1;
            int valueOne = 0, valueTwo = 0, value;
            int carry = 0, digitsNewNumber = 0;

            //Projede vsechny cislice
            while ((coupleOne < length1 || coupleTwo < length2) && (i <= currentOne.Length || j <= currentTwo.Length))
            {
                if (currentOne.Length - i >= 0 && coupleOne != length1)
                    valueOne = currentOne[currentOne.Length - i] - 48; 

                if (currentTwo.Length - j >= 0 && coupleTwo != length2 && digitsNasobitel == 0)
                    valueTwo = currentTwo[currentTwo.Length - j] - 48;

                value = valueTwo + valueOne + carry; //Pocita hodnotu cislice 


                valueOne = 0; valueTwo = 0; carry = 0;

                //Navysi zbytek pokud je soucet dvouciferny
                if (value > 9)
                {
                    carry = 1;
                    value %= 10;
                }

                number += value.ToString(); //Prida hodnotu do aktualni sady

                //Nacita novou sadu prvniho cisla, pokud se cela projela
                if (currentOne.Length - i == 0 && coupleOne < length1)
                {
                    coupleOne++;
                    if (coupleOne != length1)
                    {
                        currentOne = numberOne[coupleOne].ToString();
                        if (currentOne.Length > 9)
                        {
                            while (currentOne[remove] == '1')
                            {
                                remove++;
                            }
                            currentOne = currentOne.Remove(0, remove);
                            remove = 0;
                        }
                    }
                    i = 1;
                }
                else
                {
                    i++;
                }

                //Nacita novou sadu druheho cisla, pokud se cela projela a pokud se pricetly vsechny nuly
                if (currentTwo.Length - j == 0 && coupleTwo < length2 && digitsNasobitel == 0)
                {
                    coupleTwo++;
                    if (coupleTwo != length2)
                    {
                        currentTwo = numberTwo[coupleTwo].ToString();
                        //Uprava nove sady
                        if (currentTwo.Length > 9)
                        {
                            while (currentTwo[remove] == '1')
                            {
                                remove++;
                            }
                            currentTwo = currentTwo.Remove(0, remove);
                            remove = 0;
                        }
                    }
                    j = 1;
                }
                else if (digitsNasobitel == 0)
                {
                    j++;
                }
                else
                {
                    digitsNasobitel--;
                }

                //Uklada novou sadu, pokud je naplnena deviti cislicemi, nebo je konec scitani
                if (number.Length == 9 || (coupleOne == length1 && coupleTwo == length2))
                {

                    //Pridava zbytek k sade, pokud je konec scitani 
                    if (coupleOne == length1 && coupleTwo == length2 && carry == 1)
                    {

                        if (number.Length != 9)
                            number += "1";
                        else
                        {
                            char[] charz = new char[number.Length];
                            //Obraci cislo do tvaru "zleva doprava"
                            for (int tmp1 = number.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                            {
                                charz[tmp2] = number[tmp1];
                            }
                            number = new String(charz);

                            //Pridava jednicky na zacatek, pokud je na zacatku nula
                            if (number[0] == '0')
                            {
                                while (tmp.Length != 10 - number.Length)
                                {
                                    tmp += "1";
                                }
                                number = tmp + number;
                                tmp = "";
                            }

                            newNumber[DigitsToNines(digitsNewNumber)] = int.Parse(number); //Uklada novou sadu do cisla
                            number = "1";
                            digitsNewNumber += 9;
                        }
                        NumberOfNines[number1] = DigitsToNines(digitsNewNumber) + 1;
                    }

                    //Pridava jednicky na zacatek, pokud je na zacatku nula
                    if (number[number.Length - 1] == '0')
                    {
                        while (tmp.Length != 10 - number.Length)
                        {
                            tmp += "1";
                        }
                        number = number + tmp;
                        tmp = "";
                    }

                    //Obraci cislo do tvaru "zleva doprava"
                    char[] chars = new char[number.Length];
                    for (int tmp1 = number.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                    {
                        chars[tmp2] = number[tmp1];
                    }
                    number = new String(chars);

                    newNumber[DigitsToNines(digitsNewNumber)] = int.Parse(number); //Uklada novou sadu do cisla
                    
                    if(number.Length != 10)
                    {
                        digitsNewNumber += number.Length;
                    }
                    else
                    {
                        digitsNewNumber += 9;
                    }

                    number = "";
                }
            }
            //Uklada soucet do obecneho listu
            if (vstupniRetezec == 0)
            {
                list[number1] = newNumber;
                NumberOfNines[number1] = DigitsToNines(digitsNewNumber);
                Listdigits[number1] = digitsNewNumber;
            }
            //Uklada soucet do listu nasobeni
            else
            {
                nasobeni[0] = newNumber;
                nasobeniDigits[0] = digitsNewNumber;
            }

            return number1; //Vraci pozici souctu
        }

        /// <summary>
        /// Vynasobi cisla z listu na pozicich number1, number2
        /// </summary>
        /// <param name="number1"></param>
        /// <param name="number2"></param>
        /// <returns>Vraci index soucinu v obecnem listu</returns>
        private int Nasob(int number1, int number2)
        {
            //Nasobene cislo
            int[] numberOne = list[number1];
            //Nasobitel
            int[] numberTwo = list[number2];

            //Urci pribliznou delku noveho cisla, ktere vytvori
            int lengthNewNumber = DigitsToNines(Listdigits[number1] + Listdigits[number2]);
            int[] newNumber = new int[lengthNewNumber];
            lengthNewNumber = 0; //Promenna bude vyuzita pro konkretni urceni poctu cislic noveho cisla

            string number = ""; // Do number se uklada aktualni cislo
            int remove = 0;
            double length1 = DigitsToNines(Listdigits[number1]); //Delka prvniho cisla v sadach
            int coupleOne = 0;
            double length2 = DigitsToNines(Listdigits[number2]); //Delka druheho cisla v sadach
            int coupleTwo = 0;

            string currentOne = numberOne[0].ToString();
            //Upravi sadu pokud je na zacatku nula
            if (currentOne.Length == 10)
            {
                while (currentOne[remove] == '1')
                {
                    remove++;
                }
                currentOne = currentOne.Remove(0, remove);
                remove = 0;
            }

            string currentTwo = numberTwo[0].ToString();
            //Upravi sadu pokud je na zacatku nula
            if (currentTwo.Length == 10)
            {
                while (currentTwo[remove] == '1')
                {
                    remove++;
                }
                currentTwo = currentTwo.Remove(0, remove);
                remove = 0;
            }

            int poradiNasobitele = 0;
            int i = 1;
            int j = 2;
            bool first = true;
            int a = currentTwo[currentTwo.Length - 1] - 48;
            string tmp = "";

            //Preskoci nasobeni nulou
            if (a == 0)
            {
                while (a == 0)
                {
                    if (currentTwo.Length - j + 1 == 0 && coupleTwo < length2)
                    {
                        coupleTwo++;
                        if (coupleTwo != length2)
                        {
                            currentTwo = numberTwo[coupleTwo].ToString();
                            //Upravi novou sadu pro nuly na zacatku
                            if (currentTwo.Length == 10)
                            {
                                while (currentTwo[remove] == '1')
                                {
                                    remove++;
                                }
                                currentTwo = currentTwo.Remove(0, remove);
                                remove = 0;
                            }
                            j = 1;
                            a = currentTwo[currentTwo.Length - j] - 48;
                        }
                    }
                    if (currentTwo.Length - j >= 0 && coupleTwo != length2)
                    {
                        a = currentTwo[currentTwo.Length - j] - 48;
                    }
                    poradiNasobitele++;
                    j++;
                }
                first = false;
                nasobeni[0] = new int[] { 0 };
                nasobeniDigits[0] = 1;
            }

            int carry = 0, value;
            int digits = 0;

            int digitsNasobenec = 0;


            //Projede vsechny cislice
            while ((coupleOne < length1 || coupleTwo < length2) && (i <= currentOne.Length + 1 || j <= currentTwo.Length + 1))
            {

                coupleOne = 0;
                //Ulozi do currentOne prvni sadu z prvniho cisla
                currentOne = numberOne[0].ToString();
                if (currentOne.Length == 10)
                {
                    while (currentOne[remove] == '1')
                    {
                        remove++;
                    }
                    currentOne = currentOne.Remove(0, remove);
                    remove = 0;
                }
                i = 1;
                digits = 0;
                digitsNasobenec = 0;


                //Tento segment reprezentuje nasobeni celeho prvniho cisla(numberOne) cislici z druheho cisla(a)
                while (coupleOne < length1)
                {
                    value = a * (currentOne[currentOne.Length - i] - 48) + carry; //Nasobeni jednotlivych cislic
                    digitsNasobenec++;
                    carry = 0;

                    //Upravi zbytek, pokud je hodnota vetsi nez 9
                    if (value > 9)
                    {
                        carry = value / 10;
                        value %= 10;
                    }

                    number += value.ToString(); //Prida hodnotu do aktualni sady

                    //Aktualizuje sadu nasobence, pokud se cela projela
                    if (currentOne.Length - i == 0 && coupleOne < length1)
                    {
                        coupleOne++;
                        if (coupleOne != length1)
                        {
                            currentOne = numberOne[coupleOne].ToString();
                            if (currentOne.Length == 10)
                            {
                                while (currentOne[remove] == '1')
                                {
                                    remove++;
                                }
                                currentOne = currentOne.Remove(0, remove);
                                remove = 0;
                            }
                            i = 1;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }

                    //Zapisuje sadu, pokud je plna, nebo pokud je konec nasobeni prvniho cisla jednou cislici druheho cisla
                    if (number.Length == 9 || coupleOne == length1)
                    {
                        //Zapise zbytek do cisla, pokud je konec dilciho nasobeni
                        if (coupleOne == length1 && carry > 0)
                        {
                            if (number.Length != 9)
                                number += carry.ToString();

                            else
                            {
                                char[] charz = new char[number.Length];
                                for (int tmp1 = number.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                                {
                                    charz[tmp2] = number[tmp1];
                                }
                                number = new String(charz);
                                if (number[0] == '0')
                                {
                                    while (tmp.Length != 10 - number.Length)
                                    {
                                        tmp += "1";
                                    }
                                    number = tmp + number;
                                    tmp = "";
                                }
                                newNumber[DigitsToNines(digits)] = int.Parse(number);
                                digits += 9;
                                number = carry.ToString();

                            }
                            NumberOfNines[number1] = DigitsToNines(digits) + 1;
                            carry = 0;
                        }

                        //Obraceni cisla pomoci charu
                        char[] chars = new char[number.Length];
                        for (int tmp1 = number.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                        {
                            chars[tmp2] = number[tmp1];
                        }
                        number = new String(chars);

                        //Prida jednicky, pokud vyjde na zacatku sady nula
                        if (number[0] == '0')
                        {
                            while (tmp.Length != 10 - number.Length)
                            {
                                tmp += "1";
                            }
                            number = tmp + number;
                            tmp = "";
                        }

                        if(DigitsToNines(digits) > 0)
                        {
                            int tmp_nines = DigitsToNines(digits) - 1;
                            int tmp_length = newNumber[tmp_nines].ToString().Length;
                            string tmp_instance = newNumber[DigitsToNines(digits) - 1].ToString();
                            if (tmp_length == 10)
                            {
                                while(tmp_instance[remove] == '1')
                                {
                                    remove++;
                                }

                                if(remove > 1)
                                {
                                    if (remove - 1 < number.Length)
                                    {
                                        tmp_instance = number.Remove(0, number.Length - remove - 1) + tmp_instance.Remove(0, remove);
                                        number = number.Remove(number.Length - remove - 1, number.Length);
                                        digits += remove - 1;
                                    }
                                    else
                                    {
                                        tmp_instance = number + tmp_instance.Remove(0, remove);
                                        digits += number.Length;
                                        number = "";
                                    }
                                }
                            }
                            else if(tmp_length < 9)
                            {
                                if (number.Length > 9 - tmp_length)
                                {
                                    tmp_instance = number.Remove(0, number.Length + tmp_length - 9) + tmp_instance;
                                    digits += 9 - tmp_length;
                                    number = number.Remove(number.Length + tmp_length - 9, number.Length);
                                }
                                else
                                {
                                    tmp_instance = number + tmp_instance;
                                    digits += number.Length;
                                    number = "";
                                }
                            }
                            newNumber[tmp_nines] = int.Parse(tmp_instance);
                            remove = 0;
                        }
                        if (number != "")
                        {
                            newNumber[DigitsToNines(digits)] = int.Parse(number); //Prida sadu do noveho cisla
                        }
                        if (number.Length == 10)
                        {
                            while (number[remove] == '1')
                            {
                                remove++;
                            }
                            digits += 10 - remove;
                            remove = 0;
                        }
                        else
                        {
                            digits += number.Length;
                        }

                        //Kdyz se projede cely nasobenec, da se cislo do Listu nasobeni a aktualizuje se nasobitel
                        if (coupleOne == length1)
                        {

                            //Obraceni cisla
                            int[] numeros = new int[DigitsToNines(digits)];
                            for (int tmp_ = 0; tmp_ < DigitsToNines(digits); tmp_++)
                            {
                                numeros[tmp_] = newNumber[tmp_];
                            }

                            if (!first)
                            {
                                nasobeni[1] = numeros; //Cisla se ulozi do Listu nasobeni
                                nasobeniDigits[1] = digits; //Aktualizuje se pocet cislic
                                digitsNasobenec = 0;
                                Secti(0, 1, poradiNasobitele + 1); //Udela se mezisoucet, ktery se ulozi na pozici nasobeni[0]
                            }
                            else
                            {
                                nasobeni[0] = numeros;
                                nasobeniDigits[0] = digits;
                                first = false;
                            }
                          
                            if (currentTwo.Length - j >= 0 && coupleTwo != length2)
                            {
                                a = currentTwo[currentTwo.Length - j] - 48; //Aktualizuje cislici druheho cislo, se kterou se nasobi
                                if (a == 0)
                                {
                                    //Preskoci vsechny nuly, pokud jsou v druhem cislu, a podle toho zvetsi pocet nul noveho scitance mezisouctu
                                    while (a == 0)
                                    {
                                        if (currentTwo.Length - j + 1 == 0 && coupleTwo < length2)
                                        {
                                            coupleTwo++;
                                            if (coupleTwo != length2)
                                            {

                                                //Pokud se v prubehu preskakovani nul dostane na konec sady, aktualizuje ji
                                                currentTwo = numberTwo[coupleTwo].ToString();
                                                if (currentTwo.Length == 10) //Uprava sady od jednicek na zacatku
                                                {
                                                    while (currentTwo[remove] == '1')
                                                    {
                                                        remove++;
                                                    }
                                                    currentTwo = currentTwo.Remove(0, remove);
                                                    remove = 0;
                                                }
                                                a = currentTwo[currentTwo.Length - 1] - 48;
                                                j = 1;
                                            }
                                        }
                                        else if (currentTwo.Length - j >= 0 && coupleTwo != length2)
                                        {
                                            a = currentTwo[currentTwo.Length - j] - 48;
                                        }
                                        poradiNasobitele++;
                                        j++;
                                    }
                                }
                                else
                                {
                                    poradiNasobitele++;
                                    j++;
                                }
                            }
                            else if (currentTwo.Length - j <= 0 && coupleTwo < length2)
                            {
                                coupleTwo++;
                                if (coupleTwo != length2)
                                {
                                    currentTwo = numberTwo[coupleTwo].ToString();
                                    if (currentTwo.Length == 10)
                                    {
                                        while (currentTwo[remove] == '1')
                                        {
                                            remove++;
                                        }
                                        currentTwo = currentTwo.Remove(0, remove);
                                        remove = 0;
                                    }
                                    a = currentTwo[currentTwo.Length - 1] - 48;
                                    j = 2;
                                    poradiNasobitele++;
                                    if (a == 0)
                                    {
                                        while (a == 0)
                                        {
                                            if (currentTwo.Length - j + 1 == 0 && coupleTwo < length2)
                                            {
                                                coupleTwo++;
                                                if (coupleTwo != length2)
                                                {
                                                    currentTwo = numberTwo[coupleTwo].ToString();
                                                    if (currentTwo.Length == 10)
                                                    {
                                                        while (currentTwo[remove] == '1')
                                                        {
                                                            remove++;
                                                        }
                                                        currentTwo = currentTwo.Remove(0, remove);
                                                        remove = 0;
                                                    }
                                                    a = currentTwo[currentTwo.Length - j] - 48;
                                                }
                                            }
                                            else if (currentTwo.Length - j >= 0 && coupleTwo != length2)
                                            {
                                                a = currentTwo[currentTwo.Length - j] - 48;
                                            }
                                            poradiNasobitele++;
                                            j++;
                                        }
                                    }
                                }
                                else
                                {
                                    j++;
                                }
                            }
                        }
                        number = "";
                    }
                }
            }



            //Ulozi soucin do listu a aktualizuje pocet sad
            list[number1] = nasobeni[0];
            NumberOfNines[number1] = DigitsToNines(nasobeniDigits[0]);
            Listdigits[number1] = nasobeniDigits[0];

            nasobeni[0] = null;
            nasobeni[1] = null;
            nasobeniDigits[0] = 0;
            nasobeniDigits[1] = 0;

            return number1; //Vrati index soucinu v listu
        }

        /// <summary>
        /// Deli druhe zadane cislo(indexDelenec) prvnim zadanym cislem(indexDelitel)
        /// </summary>
        /// <param name="indexDelitel"></param>
        /// <param name="indexDelenec"></param>
        /// <param name="vstupniRetezec"></param>
        /// <returns>Vraci index v listu, kde je ulozen podil</returns>
        private int Divide(int indexDelitel, int indexDelenec, int vstupniRetezec)
        {
            int tmp;

            tmp = Bigger(indexDelenec, indexDelitel, 0);
            
            //Pokud je delitel vetsi, zapise do listu vysledek 0 a vrati na nej odkaz
            if (tmp == 1)
            {
                list[indexDelenec] = null;
                list[indexDelenec] = new int[1] { 0 };
                return indexDelenec;
            }

            //Pokud jsou cisla stejna, zapise do listu vysledek 1 a vrati na nej odkaz
            if (tmp == 2)
            {
                list[indexDelenec] = null;
                list[indexDelenec] = new int[1] { 1 };
                NumberOfNines[indexDelenec] = 1;
                Listdigits[indexDelenec] = 1;
                return indexDelenec;
            }


            int[] delenec = list[indexDelenec]; 
            int[] delitel = list[indexDelitel];

            int digitsDelenec = Listdigits[indexDelenec];
            int digitsDelitel = Listdigits[indexDelitel];
            int posunuti;
            int bigger = 0;

            //Vrati zpravu "Nelze delit nulou." pokud se uzivatel pokousel delit nulou a vypise nulu
            if (delitel[DigitsToNines(digitsDelitel) - 1].ToString().Length == 10)
            {
                System.Windows.Forms.MessageBox.Show("You can't divide by zero. Input different number.");
                list[indexDelenec] = null;
                list[indexDelenec] = new int[1] {0};
                return indexDelenec;
            }

            //Nastavi pocatecni hodnoty do listu deleni
            deleni[0] = delenec;
            deleniDigits[0] = digitsDelenec;
            deleni[1] = delitel;
            deleniDigits[1] = digitsDelitel;

            //Urci stupen radu, o ktery se zvetsuje delitel
            posunuti = digitsDelenec - digitsDelitel + 1;
            deleniDigits[1] += posunuti - 1;

            //Nastavi pocatecni hodnoty do listu nasobeni, ktere slouzi pro mezisoucty daneho podilu
            nasobeni[0] = new int[1] { 0 };
            nasobeniDigits[0] = 1;
            nasobeni[1] = new int[1] { 1 };
            nasobeniDigits[1] = 1;

            /*Odcita delitele o danem radu s klesajici tendenci, dokud
              neni delitel v zakladnim tvaru a vetsi nez odecet delence*/
            while (bigger != 1 || posunuti > 0)
            {

                if (Substract(0, 1, posunuti) == -1)
                {
                    posunuti--; //Pokud je delitel vetsi, snizi rad delitele
                    deleniDigits[1]--;
                }
                else
                {
                    bigger = Bigger(0, 1, 2); 
                    tmp = posunuti;
                    Secti(0, 1, tmp); //Provadi mezisoucet podilu pomoci ukladani do pomocneho listu ukladani

                    if (bigger == 1) //Pokud je delitel vetsi, snizi rad delitele
                    {
                        posunuti--;
                        deleniDigits[1]--;
                    }

                }

            }

            //Zapise vysledek do listu a aktualizuje prislusny pocet sad
            list[indexDelenec] = nasobeni[0];
            Listdigits[indexDelenec] = nasobeniDigits[0];
            NumberOfNines[indexDelenec] = DigitsToNines(nasobeniDigits[0]);

            return indexDelenec; //Vrati index daneho podilu v listu
        }

        /// <summary>
        /// Odecte od cisla na indexu indexMensence cislo na indexu indexMensitele
        /// </summary>
        /// <param name="indexMensence">index mensence v Listu nebo v deleni</param>
        /// <param name="indexMensitele">indec mensitele v Listu nebo v nasobeni</param>
        /// <param name="vstupniRetezec">urcuje zdroj cisel (0 = List; vetsi nez nula = deleni) a stanovuje posunutí</param>
        /// <returns></returns>
        private int Substract(int indexMensence, int indexMensitele, int vstupniRetezec)
        {
            int indexSady1 = 0;
            int indexSady2 = 0;
            int[] mensenec;
            int[] mensitel;
            int delkaNovehoCisla;
            int[] rozdil;
            int delka1;
            int delka2;
            int cisliceDelitel = 0;
            int zbaveniSeJednicek = 0;
            string tmp = "";
            int vetsi;

            //Rozhoduje z jakeho zdroje ma bigger evaluovat tato dve cisla
            if (vstupniRetezec > 0)
            {
                vetsi = Bigger(indexMensence, indexMensitele, 2);
                cisliceDelitel = vstupniRetezec - 1;
            }
            else
            {
                vetsi = Bigger(indexMensitele, indexMensence, 0);
            }


            if(vetsi == 1)
            {

                //Pokud bude mensitel vetsi a data se nacitaji z listu, vyhodi chybu
                if(vstupniRetezec == 0)
                {
                    System.Windows.Forms.MessageBox.Show("The calculation results in negative number. Please change your input.");
                    list[indexMensitele] = null;
                    list[indexMensitele] = new int[1] { 0 };
                    NumberOfNines[indexMensitele] = 1;
                    return indexMensitele;
                }

                //Pokud bude mensitel a data se nacitaji z deleni, 
                //vraci -1 jako signal pro metodu deleni, ze je druhe cislo vetsi
                else
                {
                    return -1;
                }
            }

            //Pokud budou cisla totozna, vraci nulu
            if(vetsi == 2)
            {
                if(vstupniRetezec == 0)
                {
                    list[indexMensitele] = new int[1] { 0 };
                    NumberOfNines[indexMensitele] = 1;
                }
                else
                {
                    deleni[0] = new int[] { 0 };
                    deleniDigits[0] = 1;
                }
                return indexMensitele;
            }

            //Nacita cisla z listu nebo z nasobeni, pokud je mensenec vetsi
            if (vstupniRetezec == 0)
            {
                mensenec = list[indexMensitele];
                mensitel = list[indexMensence];
                delkaNovehoCisla = Math.Max(NumberOfNines[indexMensitele] + 1, NumberOfNines[indexMensence] + 1);
                rozdil = new int[delkaNovehoCisla];
                delka1 = NumberOfNines[indexMensitele];
                delka2 = NumberOfNines[indexMensence];
            }
            else
            {
                mensitel = deleni[1];
                mensenec = deleni[0];
                rozdil = new int[DigitsToNines(deleniDigits[0]) + 1];
                delka2 = mensitel.Length;
                delka1 = DigitsToNines(deleniDigits[0]);
            }



            string sadaMensence = mensenec[0].ToString();
            //Zjistuje jestli na konci nema byt jednicka
            if (sadaMensence.Length > 9)
            {
                while (sadaMensence[zbaveniSeJednicek] == '1')
                {
                    zbaveniSeJednicek++;
                }
                sadaMensence = sadaMensence.Remove(0, zbaveniSeJednicek);
                zbaveniSeJednicek = 0;
            }
            string sadaMensitele = mensitel[0].ToString();
            if (sadaMensitele.Length > 9)
            {
                while (sadaMensitele[zbaveniSeJednicek] == '1')
                {
                    zbaveniSeJednicek++;
                }
                sadaMensitele = sadaMensitele.Remove(0, zbaveniSeJednicek);
                zbaveniSeJednicek = 0;
            }

            string novaSada = "";
            int i = 1, j = 1;
            int hodnota_cislo1 = 0, hodnota_cislo2 = 0, hodnota;
            int zbytek = 0, nines = 0;
            int poceCislicNoveCislo = 0;

            //Projizdi vsechna cisla
            while ((indexSady1 < delka1 || indexSady2 < delka2) && (i <= sadaMensence.Length || j <= sadaMensitele.Length))
            {

                //Nacita hodnoty pokud nedojel na konec cisla 
                if (sadaMensence.Length - i >= 0 && indexSady1 != delka1)
                    hodnota_cislo1 = sadaMensence[sadaMensence.Length - i] - 48;
                if (sadaMensitele.Length - j >= 0 && indexSady2 != delka2 && cisliceDelitel == 0)
                    hodnota_cislo2 = sadaMensitele[sadaMensitele.Length - j] - 48;
                poceCislicNoveCislo++;

                hodnota = hodnota_cislo1 - hodnota_cislo2 - zbytek;
                hodnota_cislo1 = 0;
                hodnota_cislo2 = 0;
                zbytek = 0;

                //Zjistuje jestli se neodcitalo vetsi cislo od mensiho
                if (hodnota < 0)
                {
                    hodnota += 10;
                    zbytek = 1;
                }

                novaSada += hodnota.ToString();

                //Nacita dalsi sadu deviti cislic mensence
                if (sadaMensence.Length - i == 0 && indexSady1 < delka1)
                {
                    indexSady1++;
                    if (indexSady1 != delka1)
                    {
                        sadaMensence = mensenec[indexSady1].ToString();

                        //Uprava nove sady
                        if (sadaMensence.Length > 9)
                        {
                            while (sadaMensence[zbaveniSeJednicek] == '1')
                            {
                                zbaveniSeJednicek++;
                            }
                            sadaMensence = sadaMensence.Remove(0, zbaveniSeJednicek);
                            zbaveniSeJednicek = 0;
                        }
                    }
                    i = 1;
                }
                else
                {
                    i++;
                }

                //Nacita dalsi sadu deviti cislic mensitele
                if (cisliceDelitel == 0)
                {
                    if (sadaMensitele.Length - j == 0 && indexSady2 < delka2)
                    {
                        indexSady2++;
                        if (indexSady2 != delka2)
                        {
                            sadaMensitele = mensitel[indexSady2].ToString();

                            //Uprava nove sady
                            if (sadaMensitele.Length > 9)
                            {
                                while (sadaMensitele[zbaveniSeJednicek] == '1')
                                {
                                    zbaveniSeJednicek++;
                                }
                                sadaMensitele = sadaMensitele.Remove(0, zbaveniSeJednicek);
                                zbaveniSeJednicek = 0;
                            }
                        }
                        j = 1;
                    }
                    else
                    {
                        j++;
                    }
                }
                else
                {
                    cisliceDelitel--;
                }

                //Zapisuje cislo pokud se naplnilo 9 cislic nebo pokud je konec odcitani
                if (novaSada.Length == 9 || (indexSady1 == delka1 && indexSady2 == delka2))
                {

                    //Osetruje zbyvajici carry, pokud uz je konec odcitani
                    if (indexSady1 == delka1 && indexSady2 == delka2 && zbytek == 1)
                    {
                        if (novaSada.Length != 9)
                            novaSada += "1";
                        else
                        {

                            //Obracu sadu do normalniho tvaru
                            char[] charz = new char[novaSada.Length];
                            for (int tmp1 = novaSada.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                            {
                                charz[tmp2] = novaSada[tmp1];
                            }
                            novaSada = new String(charz);

                            rozdil[nines] = int.Parse(novaSada);
                            //Console.WriteLine("{0}", newNumber[nines]);
                            nines++;
                            novaSada = "1";
                        }
                        NumberOfNines[indexMensitele] = nines + 1;
                    }

                    //Osetruje sady zacinajici nulou 
                    if (novaSada[novaSada.Length - 1] == '0')
                    {
                        while (tmp.Length != 10 - novaSada.Length)
                        {
                            tmp += "1";
                        }
                        novaSada = novaSada + tmp;
                        tmp = "";
                    }

                    //Obraci sadu do normalniho tvaru
                    char[] chars = new char[novaSada.Length];
                    for (int tmp1 = novaSada.Length - 1, tmp2 = 0; tmp1 >= 0; --tmp1, ++tmp2)
                    {
                        chars[tmp2] = novaSada[tmp1];
                    }
                    novaSada = new String(chars);

                    rozdil[nines] = int.Parse(novaSada);
                    //Console.WriteLine("{0}", newNumber[nines]);
                    nines++;
                    novaSada = "";
                }
            }

            //Odstrani nuly, pokud zustaly ze zacatku cisla, podle toho opravi pocet cislic
            if (rozdil[nines - 1].ToString().Length == 10)
            {
                int tmp_digits = 0;
                sadaMensence = rozdil[nines - 1].ToString();
                while (sadaMensence[zbaveniSeJednicek] == '1')
                {
                    zbaveniSeJednicek++;
                }
                sadaMensence = sadaMensence.Remove(0, zbaveniSeJednicek);
                zbaveniSeJednicek = 0;

                while (nines > 0 && sadaMensence[zbaveniSeJednicek] == '0')
                {
                    if (zbaveniSeJednicek == sadaMensence.Length - 1)
                    {
                        rozdil[nines - 1] = 0;
                        nines--;
                        tmp_digits++;
                        zbaveniSeJednicek = 0;
                        if (nines > 0)
                        {
                            sadaMensence = rozdil[nines - 1].ToString();
                            if (sadaMensence.Length == 10)
                            {
                                while (sadaMensence[zbaveniSeJednicek] == '1')
                                {
                                    zbaveniSeJednicek++;
                                }
                                sadaMensence = sadaMensence.Remove(0, zbaveniSeJednicek);
                                zbaveniSeJednicek = 0;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        zbaveniSeJednicek++;
                        tmp_digits++;
                    }
                }
                if (nines == 0)
                {
                    nines = 1;
                }

                if (zbaveniSeJednicek > 0)
                {
                    rozdil[nines - 1] = int.Parse(sadaMensence.Remove(0, zbaveniSeJednicek));
                }


                poceCislicNoveCislo -= tmp_digits;
            }


            //Console.WriteLine("{0}", digitsNewNumber);

            if (vstupniRetezec == 0)
            {
                list[indexMensitele] = rozdil;
                NumberOfNines[indexMensitele] = nines;
            }
            else
            {
                deleni[0] = rozdil;
                deleniDigits[0] = poceCislicNoveCislo;
            }

            return indexMensitele;
        }
        /// <summary>
        /// Převádí počet číslic na počet sad
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        private int DigitsToNines(int digits)
        {
            while (digits % 9 != 0)
            {
                digits++;
            }
            return digits / 9;
        }

        /// <summary>
        /// Porovnava dve cisla, pokud je prvni vetsi, vraci 0; pokud je druhe vetsi, vraci 1; pokud se rovnaji, vraci 2
        /// </summary>
        /// <param name="number1">prvni cislo</param>
        /// <param name="number2">druhe cislo</param>
        /// <param name="vstupniRetezec">definuje seznam, ze ktereho se budou cisla nacitat: 0 pro list, 1 pro nasobeni, 2 pro deleni</param>
        /// <returns></returns>
        private int Bigger(int number1, int number2, int vstupniRetezec)
        {
            int length1;
            int length2;
            int[] numberOne;
            int[] numberTwo;
            int nines;
            int coupleOne;
            int coupleTwo;
            int realCoupleTwo;


            if (vstupniRetezec == 0)
            {
                numberOne = list[number1];
                numberTwo = list[number2];
                length1 = Listdigits[number1];
                length2 = Listdigits[number2];
                nines = DigitsToNines(Listdigits[number1]);
                coupleOne = DigitsToNines(Listdigits[number2]);
                coupleTwo = DigitsToNines(Listdigits[number1]);
            }
            else if (vstupniRetezec == 1)
            {
                numberOne = nasobeni[0];
                numberTwo = nasobeni[1];
                length1 = nasobeniDigits[0];
                length2 = nasobeniDigits[1];
                nines = DigitsToNines(nasobeniDigits[0]);
                coupleOne = DigitsToNines(nasobeniDigits[0]);
                coupleTwo = DigitsToNines(nasobeniDigits[1]);
            }
            else
            {
                numberOne = deleni[0];
                numberTwo = deleni[1];
                length1 = deleniDigits[0];
                length2 = deleniDigits[1];
                nines = DigitsToNines(deleniDigits[1]);
                coupleOne = DigitsToNines(deleniDigits[0]);
                coupleTwo = DigitsToNines(deleniDigits[1]);

            }
            realCoupleTwo = numberTwo.Length;

            if (length1 > length2)
            {
                return 0; //Vraci 0 pokud je prvni cislo vetsi
            }
            else if (length1 < length2)
            {
                return 1; // Vraci 1 pokud je druhe cislo vetsi
            }
            else
            {
                int i = 0;

                int remove = 0;


                string currentOne = numberOne[coupleOne - 1].ToString();
                //Uprava sady pokud je na zacatku 0
                if (currentOne.Length > 9)
                {
                    while (currentOne[remove] == '1')
                    {
                        remove++;
                    }
                    currentOne = currentOne.Remove(0, remove);
                    remove = 0;
                }

                string currentTwo = numberTwo[realCoupleTwo - 1].ToString();
                //Uprava sady pokud je na zacatku 0
                if (currentTwo.Length > 9)
                {
                    while (currentTwo[remove] == '1')
                    {
                        remove++;
                    }
                    currentTwo = currentTwo.Remove(0, remove);
                    remove = 0;
                }

                while (coupleOne > 0 && currentOne.Length != 1 && coupleOne!= 1 && i != 1)
                {
                    if (currentTwo.Length - 1 < i)
                    {
                        currentTwo += "0";
                    }
                    if (currentOne[i] > currentTwo[i])
                    {
                        return 0;
                    }

                    if (currentOne[i] < currentTwo[i])
                    {
                        return 1;
                    }

                    if (currentOne.Length - i <= 1 && coupleOne > 0)
                    {
                        coupleOne--;
                        coupleTwo--;
                        realCoupleTwo--;
                        if (coupleOne > 0)
                        {
                            currentOne = numberOne[coupleOne - 1].ToString();
                            if (currentOne.Length > 9)
                            {
                                while (currentOne[remove] == '1')
                                {
                                    remove++;
                                }
                                currentOne = currentOne.Remove(0, remove);
                                remove = 0;
                            }
                        }
                        else if (i + 1 == currentOne.Length)
                        {
                            return 2;
                        }

                        if (realCoupleTwo > 0)
                        {
                            currentTwo = numberTwo[realCoupleTwo - 1].ToString();
                            if (currentTwo.Length > 9)
                            {
                                while (currentTwo[remove] == '1')
                                {
                                    remove++;
                                }
                                currentTwo = currentTwo.Remove(0, remove);
                                remove = 0;
                            }
                        }
                        else
                        {
                            currentTwo = "000000000";
                        }
                        i = 1;
                    }
                    else
                    {
                        i++;
                    }
                }
                return 2;
            }
        }
    }
}