using System;

class Program
{
    // kiểm tra một ký tự có phải là ngoặc mở hay không
    static bool IsOpening(char c)
    {
        return c == '(' || c == '[' || c == '{';
    }

    // kiểm tra ngoặc mở và ngoặc đóng có cùng loại hay không
    static bool IsMatching(char open, char close)
    {
        return (open == '(' && close == ')') ||
               (open == '[' && close == ']') ||
               (open == '{' && close == '}');
    }

    // tìm vị trí lỗi đầu tiên trong chuỗi, trả về -1 nếu chuỗi hợp lệ
    static int FindError(string s)
    {
        // lưu ký tự ngoặc mở và vị trí của nó để đối chiếu khi gặp ngoặc đóng
        Stack<(char ch, int pos)> st = new Stack<(char ch, int pos)>();

        // duyệt lần lượt từng ký tự để mỗi ký tự chỉ cần được xét một lần
        for (int i = 0; i < s.Length; i++)
        {
            // lấy ký tự hiện tại để kiểm tra loại ngoặc
            char c = s[i];

            // nếu là ngoặc mở thì lưu vào Stack để chờ ngoặc đóng tương ứng
            if (IsOpening(c))
            {
                st.Push((c, i));
            }
            // nếu là ngoặc đóng thì cần tìm ngoặc mở tương ứng trên đỉnh Stack
            else if (c == ')' || c == ']' || c == '}')
            {
                // nếu Stack rỗng thì không có ngoặc mở nào để ghép với ngoặc đóng hiện tại
                if (st.Count == 0)
                {
                    return i;
                }

                // lấy ngoặc mở gần nhất vì ngoặc mở sau cùng phải được đóng trước
                var top = st.Pop();

                // nếu hai ngoặc không cùng loại thì vị trí ngoặc đóng hiện tại là vị trí lỗi đầu tiên
                if (!IsMatching(top.ch, c))
                {
                    return i;
                }
            }

            // các ký tự không phải ngoặc được bỏ qua vì không ảnh hưởng đến tính cân bằng
        }

        // nếu còn ngoặc mở thì tìm vị trí nhỏ nhất trong số các ngoặc chưa được đóng
        if (st.Count > 0)
        {
            // khởi tạo vị trí lỗi bằng giá trị lớn để tìm vị trí nhỏ nhất
            int firstUnclosed = int.MaxValue;

            // duyệt các ngoặc còn lại để tìm ngoặc mở chưa đóng xuất hiện sớm nhất
            foreach (var item in st)
            {
                // cập nhật vị trí lỗi nếu tìm thấy ngoặc mở xuất hiện sớm hơn
                if (item.pos < firstUnclosed)
                {
                    firstUnclosed = item.pos;
                }
            }

            // trả về vị trí ngoặc mở chưa đóng sớm nhất
            return firstUnclosed;
        }

        // không còn ngoặc mở và không có lỗi khi duyệt nên chuỗi hợp lệ
        return -1;
    }

    static void Main1()
    {
        // đọc nguyên một dòng để xử lý cả chuỗi rỗng và chuỗi có ký tự thông thường
        string s = Console.ReadLine() ?? "";

        // gọi hàm kiểm tra để lấy vị trí lỗi hoặc nhận giá trị -1 nếu hợp lệ
        int errorPos = FindError(s);

        // in thông báo hợp lệ nếu không tìm thấy lỗi
        if (errorPos == -1)
        {
            Console.WriteLine("HOP LE");
        }
        // in vị trí lỗi đầu tiên nếu chuỗi không hợp lệ
        else
        {
            Console.WriteLine("SAI tai vi tri " + errorPos);
        }
    }
}