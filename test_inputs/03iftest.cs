/* Tests simple comparisons and if statements */

public class iftest {

    public static void main() {
        int i, j;
        i = 0;
        j = 1;
        WriteLine("TCCL if test");
        if ( i < 10 ) 
            WriteLine("In then part");
        else  i = j; 
        if ( i == 10 ) j = i;
        else  WriteLine("In else part"); 
    }
}

