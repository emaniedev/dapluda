// See https://aka.ms/new-console-template for more information

var pluginPath = "2testplugin.cs.test";

static List<Token> extractParameteresFromFunction(List<Token> tokens, int i)
{
    var subtokens = new List<Token>();
    int j = i + 1;
    while (tokens[j].type != tokenType.CLOSE_PAREN)
    {
        subtokens.Add(tokens[j]);
        j++;
    }
    subtokens.Add(tokens[j]);
    return subtokens;
}

string[] lookUpCombinations = {
    "new", "Entity"
};

List<Token> tokens = new List<Token>();
tokenizado();
newEntityDependency();

List<Token> posibleEntitySymbols = new List<Token>();


for (int i = 0; i < tokens.Count; i++)
{
    var current = tokens[i];
    if (current.symbol == "Entity" &&
        tokens[i + 1].type == tokenType.SYMBOL &&
        (tokens[i + 2].type == tokenType.SEMICOLON ||
         tokens[i + 2].type == tokenType.EQUALS))
    {
        posibleEntitySymbols.Add(tokens[i + 1]);
    }

}

foreach (var posible in posibleEntitySymbols)
{
    Console.WriteLine(posible);

    foreach (var token in tokens.Where(t => t.type == tokenType.SYMBOL))
    {
        if (token.symbol == posible.symbol)
        {
            int i = tokens.IndexOf(token);
        }
    }


}

void tokenizado()
{
    using (StreamReader reader = new StreamReader(pluginPath))
    {
        var lex = new Lexer(reader);


        var token = lex.nextToken();

        while (token != null)
        {
            if (!String.IsNullOrEmpty(token.symbol))
            {
                tokens.Add(token);
            }
            token = lex.nextToken();
        }
    }
    Console.WriteLine("Terminado tokenizado");
    Console.WriteLine("--------------------");
    Console.WriteLine($"Encontrados {tokens.Count} tokens.");
}

void newEntityDependency()
{
    foreach (var tk in tokens.Where(t => t.type == tokenType.SYMBOL && t.symbol.ToLower() == "entity"))
    {
        /* Console.WriteLine("Token encontrado:"); */
        /* Console.WriteLine(tk); */
        /* Console.WriteLine("-----------------"); */
        int i = tokens.IndexOf(tk);
        var ptk = tokens[i - 1];
        /* Console.WriteLine("Token previo:"); */
        /* Console.WriteLine(ptk); */
        /* Console.WriteLine("-----------------"); */
        if (ptk.symbol == "new")
        {
            List<Token> subtokens = extractParameteresFromFunction(tokens, i);

            Console.WriteLine("Se ha encontrado una dependencia new Entity sobre");
            foreach (var dept in subtokens)
            {
                if (dept.type == tokenType.LITERAL)
                    Console.WriteLine($"\t - {dept.symbol}");
            }
            Console.WriteLine("--------------------------------------------");
        }

    }
}



public class Lexer
{
    StreamReader reader;
    Token[] tokens;
    string[] reservedWords = {
        "abstract",
        "As",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "Char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "Extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "Goto",
        "if",
        "implicit",
        "in",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "Object",
        "operator",
        "out",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "Short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "This",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
"void"
    };
    char[] reservedPunctuation = {
        // '/',
        '.',
        '\"',
        '\'',
        '=',
        '(',
        ')',
        '[',
        ']',
        '{',
        '}',
        ';',
        ':'
    };

    public Lexer(StreamReader r)
    {
        reader = r;
    }


    public Token? nextToken()
    {
        List<char> ct = new List<char>();
        char cc;
        Token t = new Token();

        cc = chopChar();
        if (reader.EndOfStream)
        {
            Console.WriteLine("Se acabo!!");
            return null;
        }


        while (cc != -1 && !char.IsSeparator(cc) && cc != '\n' && !reservedPunctuation.Contains(cc))
        {
            if (char.IsLetter(cc) || char.IsSymbol(cc) || cc == '/')
            {
                ct.Add(cc);
            }
            cc = peekChar();
            if (!reservedPunctuation.Contains(cc))
            {
                cc = chopChar();
            }
        }
        if (ct.Count == 0 && reservedPunctuation.Contains(cc))
        {
            ct.Add(cc);

            switch (cc)
            {
                // case '/':
                //     t.type = tokenType.SLASH;
                //     break;
                case '.':
                    t.type = tokenType.DOT;
                    break;
                case '\"':
                    t.type = tokenType.LITERAL;
                    ct = ct.TakeLast(ct.Count - 1).ToList();
                    ct.AddRange(chopCharUntil('\"'));
                    break;
                case '\'':
                    t.type = tokenType.LITERAL;
                    ct.AddRange(chopCharUntil('\''));
                    break;
                case '=':
                    t.type = tokenType.EQUALS;
                    break;
                case '(':
                    t.type = tokenType.OPEN_PAREN;
                    break;
                case ')':
                    t.type = tokenType.CLOSE_PAREN;
                    break;
                case '[':
                    t.type = tokenType.OPEN_BRACKET;
                    break;
                case ']':
                    t.type = tokenType.CLOSE_BRACKET;
                    break;
                case '{':
                    t.type = tokenType.OPEN_CURLY;
                    break;
                case '}':
                    t.type = tokenType.CLOSE_CURLY;
                    break;
                case ';':
                    t.type = tokenType.SEMICOLON;
                    break;
                case ':':
                    t.type = tokenType.COLON;
                    break;
                default:
                    t.type = tokenType.SYMBOL;
                    break;
            }
        }

        if (ct.Count > 1)
        {
            if (ct[0] == '/' && ct[1] == '/')
            {
                t.type = tokenType.COMMENT;
                // Es un comentario, cogemos toda la linea

                ct.AddRange(chopCharUntil('\n'));
            }
        }

        var currentSymbol = new String(ct.ToArray());
        if (reservedWords.Contains(currentSymbol))
        {
            switch (currentSymbol)
            {
                case "using":
                    ct.Add(cc);
                    ct.AddRange(chopCharUntil('\n'));
                    t.type = tokenType.USING;
                    break;
                default:
                    t.type = tokenType.RESERVED;
                    break;
            }
        }

        t.symbol = new String(ct.ToArray());
        return t;
    }


    public char chopChar()
    {
        char charkey = (char)reader.Read();
        return charkey;
    }
    public char peekChar()
    {
        char charkey = (char)reader.Peek();
        return charkey;
    }
    public List<char> chopCharUntil(char end)
    {
        var ccs = new List<char>();
        var cc = chopChar();
        while (cc != end)
        {
            ccs.Add(cc);
            cc = chopChar();
        }
        return ccs;
    }

    public char[] readLine()
    {
        return reader.ReadLine().ToCharArray();
    }
}
public class Token
{
    public tokenType type;
    public string symbol;
    public Token[] tokens;


    public override string ToString()
    {
        return $"{type} - {symbol}";
    }
}

public enum tokenType
{
    SYMBOL,
    EQUALS,
    LITERAL,
    DOT,
    RESERVED,
    COMMENT,
    USING,
    SLASH,
    OPEN_PAREN,
    CLOSE_PAREN,
    OPEN_BRACKET,
    CLOSE_BRACKET,
    OPEN_CURLY,
    CLOSE_CURLY,
    SEMICOLON,
    COLON
}
