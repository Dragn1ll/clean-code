using System.Text;

StringBuilder sb = new StringBuilder("привет");
for (int i = 0; i < sb.Length; i++)
{
    Console.WriteLine(sb.Remove(0, 1));
    i--;
}
