// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using Microsoft.VisualBasic;
//min 27:38
while (true)
{
    //min 38:04 
    // 1 + 3 * 5
    //      +
    //     / \
    //    1   *
    //       / \       
    //      3   5
        Console.Write(">  ");
    var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            return;
        var parser = new Parser(line);
        var expression = parser.Parse();

        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        PrettyPrint(expression);

        Console.ForegroundColor = color;

        var lexer = new Lexer(line);
        while(true)
        {
            var token = lexer.NextToken();
            if(token.Kind == SyntaxKind.EndOfFileToken)
                break;
            Console.Write($"{token.Kind}: '{token.Text}'");
            if (token.Value != null)
                Console.Write($"{token.Value}");
            Console.WriteLine();         
            
        }
}

// Impresión del árbol de sintaxis abstracta
static void PrettyPrint(SyntaxNode node, string indent ="")
{
    Console.Write(node.Kind);
    if (node is SyntaxToken t && t.Value != null)
    {
        Console.Write(" ");
        Console.Write(t.Value);
    }
    indent += "    ";
    foreach (var child in node.GetChildren())
        PrettyPrint(child, indent);
}
enum SyntaxKind
{
    NumberToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpToken,
    ClToken,
    BadToken,
    EndOfFileToken,
    WhiteSpaceToken,
    NumberExpression,
    BinaryExpression
}
class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
    Kind = kind;
    Position = position;
    Text = text;
    Value = value;
    }
    public override SyntaxKind Kind {get;}
    public int Position {get;}
    public string Text {get;}
    public object Value {get;}


    public override IEnumerable<SyntaxNode> GetChildren()
    {
    return Enumerable.Empty<SyntaxNode>();
    }
}
class Lexer 
{
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
        if(_position >= _text.Length)
        {
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
        }
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
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, null);
        }
        //Validar los operadores
        if ( Current == '+')
            return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
        else if (Current == '-')
            return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
        else if (Current == '*')
            return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
        else if (Current == '/')
            return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
       else if (Current == '(')
            return new SyntaxToken(SyntaxKind.OpToken, _position++, "(", null);
         else if (Current == ')')
            return new SyntaxToken(SyntaxKind.ClToken, _position++, ")", null);
        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null );
    }
}
abstract class SyntaxNode 
{
    public abstract SyntaxKind Kind {get ;}
    //Crear una nocion de "nodo hijo" min 45:48
    public abstract IEnumerable<SyntaxNode> GetChildren();
}
abstract class ExpressionSyntax : SyntaxNode
{
   
}
sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }
    public override SyntaxKind Kind => SyntaxKind.NumberExpression;
    public SyntaxToken NumberToken {get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}
sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        //40:45
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public ExpressionSyntax Left {get; }
    public SyntaxToken OperatorToken {get; }
    public ExpressionSyntax Right {get; }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();

        var lexer = new Lexer(text);
        SyntaxToken token;
        do
        {
            token = lexer.NextToken();
            if(token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.BadToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);
        _tokens = tokens.ToArray();
    }
    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];
        return _tokens[index];
    }
    private SyntaxToken Current => Peek(0);
    //A partir de esta expresion buscaremos trabajar con la estructura del arbol
    private SyntaxToken NextToken()
    {
        var current = Current;
         _position++;
        return current;
    }
    private SyntaxToken Match(SyntaxKind kind)
    {
        if(Current.Kind == kind)
            return NextToken();
        return new SyntaxToken(kind, Current.Position, null, null);
    }
    public ExpressionSyntax Parse()
    {
       var left = ParsePrimaryExpression();
       while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
       {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
       } 
       return left;
    }
    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.NumberToken);
    return new NumberExpressionSyntax(numberToken);
    }
}