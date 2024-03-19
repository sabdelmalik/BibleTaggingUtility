using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    internal class FixedSizeStack<T>
    {
        int maxSize = 1000;

        private Stack<object> stack1 = new Stack<object>();
        private Stack<object> stack2 = new Stack<object>();

        public FixedSizeStack() 
        {
        }
        public FixedSizeStack(int size) { 
            this.maxSize = size;
        }

        public int Count
        {
            get { return stack1.Count; }
        }

        public void Push(T item)
        {
            stack1.Push(item);
            if (stack1.Count <= maxSize)
                return;

            stack2.Clear();
            // copy stack1 to stack2 
            // the first item becomes last
            while (stack1.Count > 0)
            {
                stack2.Push(stack1.Pop());
            }
            // get rid of stack1 last item
            stack2.Pop();

            // copy stack2 to stack1 
            // this restores the order leaving the top slot empty 
            while (stack2.Count > 0)
            {
                stack1.Push(stack2.Pop());    //Or Instead of 2nd while you can also swap the stack1 and stack2(like swapping two variables)
            }
        }

        public T Pop()
        {
            return (T)stack1.Pop();
        }
    }
}
