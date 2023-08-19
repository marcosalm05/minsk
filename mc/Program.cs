// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;

while (true)
{
        Console.Write(">  ");
    var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            return;
        if(line == "1 + 2 * 3" )
            Console.WriteLine("7");
        else 
            Console.WriteLine("Invalid expression!");
}

enum SyntaxKind
{
    NumberToken, 
}
class SyntaxToken
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
    Kind = kind;
    Position = position;
    Text = text;
    Value = value;
    }
    public SyntaxKind Kind {get;}
    public int Position {get;}
    public string Text {get;}
    public object Value {get;}
}
class Lexer {
    /*Esta declaración crea un campo privado llamado _text que almacena una cadena de texto. 
    La palabra clave readonly indica
    que este campo solo puede asignarse en su declaración o en el constructor de la clase, 
    y no se puede modificar después.*/
    private readonly string _text;
    /*Aquí, se declara un campo privado llamado _position que almacena un valor entero. 
    Como no tiene la palabra clave readonly, 
    su valor puede ser modificado en cualquier momento dentro de los métodos de la clase.*/
    private int _position;
    public Lexer(string text)
    {
        _text = text;
    }
    private  char Current
    {
        //descriptor de acceso
        get
        {
            if (_position>= _text.Length)
                return '\0';
            return _text[_position];
        }
    }
    private void Next()
    {
        _position++;
    }
    //Evaluador de expresión
    public SyntaxToken NextToken()
    {
        //  <numbers>
        // + - * / ( )
        // <whitespace>
        
        //min 23:11
        if(char.IsDigit(Current))
        {
            var start = _position;
            while(char.IsDigit(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);
            int.TryParse(text, out var value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if(char.IsWhiteSpace(Current))
        {
             var start = _position;
            while(char.IsWhiteSpace(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);
            int.TryParse(text, out var value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }
        //Validar los operadores
        if(Current == '+')
        {

        }
    }
}