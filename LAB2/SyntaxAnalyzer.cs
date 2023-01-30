using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB2
{
    // Класс исключительных ситуаций синтаксического анализа.
    class SynAnException : Exception
    {
        // Позиция возникновения исключительной ситуации в анализируемом тексте.
        private int lineIndex; // Индекс строки.
        private int symIndex;  // Индекс символа.

        // Индекс строки, где возникла исключительная ситуация - свойство только для чтения.
        public int LineIndex
        {
            get { return lineIndex; }
        }

        // Индекс символа, на котором возникла исключительная ситуация - свойство только для чтения.
        public int SymIndex
        {
            get { return symIndex; }
        }

        // Конструктор исключительной ситуации.
        // message - описание исключительной ситуации.
        // lineIndex и symIndex - позиция возникновения исключительной ситуации в анализируемом тексте.
        public SynAnException(string message, int lineIndex, int symIndex)
            : base(message)
        {
            this.lineIndex = lineIndex;
            this.symIndex = symIndex;
        }
    }

    // Класс "Синтаксический анализатор".
    // При обнаружении ошибки в исходном тексте он генерирует исключительную ситуацию SynAnException или LexAnException.
    class SyntaxAnalyzer
    {
        private LexicalAnalyzer lexAn; // Лексический анализатор.

        // Конструктор синтаксического анализатора. 
        // В качестве параметра передается исходный текст.
        public SyntaxAnalyzer(string[] inputLines)
        {
            // Создаем лексический анализатор.
            // Передаем ему текст.
            lexAn = new LexicalAnalyzer(inputLines);
        }

        // Обработать синтаксическую ошибку.
        // msg - описание ошибки.
        private void SyntaxError(string msg)
        {
            // Генерируем исключительную ситуацию, тем самым полностью прерывая процесс анализа текста.
            throw new SynAnException(msg, lexAn.curLineIndex, lexAn.curSymIndex);
        }

        // Проверить, что тип текущего распознанного токена совпадает с заданным.
        // Если совпадает, то распознать следующий токен, иначе синтаксическая ошибка.
        private void Match(TokenKind tkn)
        {
            if (lexAn.Token.Type == tkn) // Сравниваем.
            {
                lexAn.RecognizeNextToken(); // Распознаем следующий токен.
            }
            else 
            {
                SyntaxError("Ожидалось " + tkn.ToString()); // Обнаружена синтаксическая ошибка.
            }
        }

        // Проверить, что текущий распознанный токен совпадает с заданным (сравнение производится в нижнем регистре).
        // Если совпадает, то распознать следующий токен, иначе синтаксическая ошибка.
        private void Match(string tkn)
        {
            if (lexAn.Token.Value.ToLower() == tkn.ToLower()) // Сравниваем.
            {
                lexAn.RecognizeNextToken(); // Распознаем следующий токен.
            }
            else
            {
                SyntaxError("Ожидалось " + tkn); // Обнаружена синтаксическая ошибка.
            }
        }

        // Процедура разбора для стартового нетерминала S.
        private void S()
        {
            A(); // Вызываем процедуру разбора для нетерминала A.
            C();
        }

        // Процедура разбора для нетерминала C.
        private void C()
        {
            if (lexAn.Token.Type == TokenKind.Semicolon) // Если текущий токен - ";".
            {
                lexAn.RecognizeNextToken(); // Пропускаем этот знак.
                
                S();
                C();// Вызываем процедуру разбора для нетерминала S,C.
            }
            else
            {
                //Епсилон
            }
        }

        // Процедура разбора для нетерминала A.
        private void A()
        {
            B(); // Вызываем процедуру разбора для нетерминала B.
            X();          
        }

        // Процедура разбора для нетерминала X.
        private void X()
        {
            if (lexAn.Token.Type == TokenKind.Comma) // Если текущий токен - ",".
            {
                lexAn.RecognizeNextToken(); // Пропускаем этот знак.

                B();
                X();// Вызываем процедуру разбора для нетерминала B,X.
            }
            else
            {
                //Епсилон
            }
        }

        // Процедура разбора для нетерминала B.
        private void B()
        {
            if (lexAn.Token.Type == TokenKind.Number)
            {
                lexAn.RecognizeNextToken();
            }
            else 
            {
                lexAn.RecognizeNextToken();
                Match(TokenKind.Colon);
                lexAn.RecognizeNextToken();
            }
        }

        // Провести синтаксический анализ текста.
        public void ParseText()
        {
            lexAn.RecognizeNextToken(); // Распознаем первый токен в тексте.

            S(); // Вызываем процедуру разбора для стартового нетерминала S.

            if (lexAn.Token.Type != TokenKind.EndOfText) // Если текущий токен не является концом текста.
            {
                SyntaxError("После арифметического выражения идет еще какой-то текст"); // Обнаружена синтаксическая ошибка.
            }
        }
    }
}
