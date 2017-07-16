using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shunting_yard
{
    /***
while there are tokens to be read:
	read a token.
	if the token is a number, then push it to the output queue.
	if the token is an operator, then:
		while there is an operator at the top of the operator stack with greater than or equal to precedence:
				pop operators from the operator stack, onto the output queue;
		push the read operator onto the operator stack.
	if the token is a left bracket (i.e. "("), then:
		push it onto the operator stack.
	if the token is a right bracket (i.e. ")"), then:
		while the operator at the top of the operator stack is not a left bracket:
			pop operators from the operator stack onto the output queue.
		pop the left bracket from the stack.
		//if the stack runs out without finding a left bracket, then there are
		mismatched parentheses.

if there are no more tokens to read:
	while there are still operator tokens on the stack:
        //if the operator token on the top of the stack is a bracket, then
		there are mismatched parentheses.
        pop the operator onto the output queue.
exit.
         ***/

    public class ShuntingYard
    {
        private static readonly Dictionary<string, int> Operators = new Dictionary<string, int>() { { "+", 0 }, { "-", 0 }, { "*", 1 }, { "/", 1 }, { "^", 2 } };

        public static void Main(string[] args)
        {
            const string input = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
            var array = input.Split(' ');
            string[] output = "3 4 2 * 1 5 - 2 3 ^ ^ / +".Split(' ');

            var loopOutput = LoopYard(array).Split(' ');
            var recursiveOutpue = RecursiveYard(array, "");
            var test = false;

            for (int index = 0; index < recursiveOutpue.Length; index++)
                test = loopOutput[index] == output[index] && recursiveOutpue[index].ToString() == output[index];
            

            Debug.Assert(test);

        }

        private static string LoopYard(IEnumerable<string> array)
        {
            Queue<string> queue = new Queue<string>();
            Stack<string> stack = new Stack<string>();

            foreach (string token in array)
            {
                if (int.TryParse(token, out int intToken))
                {
                    queue.Enqueue(token);
                    continue;
                }

                if (Operators.ContainsKey(token))
                {
                    while (stack.Any() && Operators.ContainsKey(stack.Peek()) && Operators[stack.Peek()] >= Operators[token] &&
                           stack.Peek() != token)
                    {
                        queue.Enqueue(stack.Pop());
                    }
                    stack.Push(token);
                    continue;
                }

                switch (token)
                {
                    case "(":
                        stack.Push(token);
                        continue;
                    case ")":
                        while (stack.Any() && stack.Peek() != "(")
                        {
                            queue.Enqueue(stack.Pop());
                        }

                        if (stack.Peek() != "(")
                            throw new Exception();

                        stack.Pop();
                        break;
                }
            }

            while (stack.Any())
            {
                queue.Enqueue(stack.Pop());
            }

            return queue.Aggregate((p, n) => $"{p} {n}");
        }

        public static string RecursiveYard(string[] array, string currentToken)
        {
            if (array.Length == 0)
                return !string.IsNullOrEmpty(currentToken) ? currentToken : "";

            var token = array[0];
            if (int.TryParse(token, out int intToken))
                return token + RecursiveYard(array.Skip(1).ToArray(), currentToken);
            

            if (Operators.ContainsKey(token))
            {
                if (!string.IsNullOrEmpty(currentToken) && Operators.ContainsKey(currentToken) && Operators[currentToken] >= Operators[token] && currentToken != token)
                {
                    return currentToken + RecursiveYard(array, token);
                }
                return RecursiveYard(array.Skip(1).ToArray(), token) + currentToken;
            }

            if (token != "(")
                return RecursiveYard(array.Skip(1).ToArray(), "");

            string[] strings = array.Skip(1).ToArray().TakeWhile(s => s != ")").ToArray();
            return RecursiveYard(strings, "") + RecursiveYard(array.SkipWhile(s => s != ")").ToArray(), currentToken);
        }
    }
}
