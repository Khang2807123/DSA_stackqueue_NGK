using System;
using System.Collections.Generic;
using System.Diagnostics;

// hàng đợi cài bằng hai ngăn xếp: ngăn nhận phần tử mới và ngăn dùng để lấy ra
public class TwoStackQueue<T>
{
    // ngăn nhận: enqueue đẩy vào đây nên đỉnh là phần tử mới nhất
    private readonly Stack<T> inbox = new Stack<T>();

    // ngăn lấy: sau khi đổ, đỉnh là phần tử cũ nhất nên dequeue chỉ việc pop
    private readonly Stack<T> outbox = new Stack<T>();

    // tổng số phần tử đã đổ từ ngăn nhận sang ngăn lấy, chỉ class này được sửa
    public long Transfers { get; private set; }

    // số phần tử của hàng đợi là tổng số phần tử của cả hai ngăn
    public int Count => inbox.Count + outbox.Count;

    // thêm phần tử vào cuối hàng, chỉ push vào ngăn nhận nên Θ(1)
    public void Enqueue(T value)
    {
        // không đụng đến ngăn lấy để không phá thứ tự các phần tử đang chờ lấy
        inbox.Push(value);
    }

    // lấy phần tử đầu hàng, Θ(1) khấu hao
    public T Dequeue()
    {
        // chỉ đổ khi ngăn lấy rỗng, nếu đổ khi còn phần tử thì thứ tự FIFO bị sai
        if (outbox.Count == 0)
        {
            // cả hai ngăn đều rỗng nghĩa là hàng rỗng, báo lỗi theo yêu cầu đề bài
            if (inbox.Count == 0) throw new InvalidOperationException("hang doi rong");

            // đổ toàn bộ ngăn nhận sang ngăn lấy, lặp đến khi ngăn nhận hết phần tử
            while (inbox.Count > 0)
            {
                // pop từ ngăn nhận rồi push sang ngăn lấy, việc này đảo ngược thứ tự nên phần tử cũ nhất lên đỉnh
                outbox.Push(inbox.Pop());

                // đếm mỗi lần đổ để chứng minh mỗi phần tử chỉ bị đổ một lần
                Transfers++;
            }
        }

        // đỉnh ngăn lấy chính là phần tử đầu hàng
        return outbox.Pop();
    }
}

// bản đối chứng: mỗi enqueue đổ toàn bộ sang ngăn phụ rồi đổ lại nên Θ(n) mỗi lần
public class NaiveTwoStack<T>
{
    // ngăn chính luôn giữ phần tử cũ nhất ở đỉnh
    private readonly Stack<T> main = new Stack<T>();

    // ngăn phụ chỉ dùng tạm khi enqueue
    private readonly Stack<T> aux = new Stack<T>();

    // số phần tử là số phần tử của ngăn chính vì ngăn phụ luôn rỗng giữa các thao tác
    public int Count => main.Count;

    // thêm phần tử vào cuối hàng bằng cách đổ đi đổ lại, tốn Θ(n)
    public void Enqueue(T value)
    {
        // đổ hết ngăn chính sang ngăn phụ để đáy ngăn chính trống chỗ đặt phần tử mới
        while (main.Count > 0) aux.Push(main.Pop());

        // đặt phần tử mới vào ngăn chính, lúc này nó nằm dưới cùng nên là phần tử cuối hàng
        main.Push(value);

        // đổ trả toàn bộ từ ngăn phụ về ngăn chính để phần tử cũ nhất lại nằm trên đỉnh
        while (aux.Count > 0) main.Push(aux.Pop());
    }

    // lấy phần tử đầu hàng, Θ(1) vì đỉnh ngăn chính đã là phần tử cũ nhất
    public T Dequeue()
    {
        // ngăn chính rỗng nghĩa là hàng rỗng, báo lỗi theo yêu cầu đề bài
        if (main.Count == 0) throw new InvalidOperationException("hang doi rong");

        // pop đỉnh ngăn chính chính là phần tử đầu hàng
        return main.Pop();
    }
}

class Bai4
{
    // in kết quả so sánh giữa giá trị mong đợi và giá trị thực tế
    static void Check(string what, object expected, object actual)
    {
        // hai giá trị bằng nhau thì ca đạt
        bool ok = Equals(expected, actual);

        // in mong đợi, thực tế và dấu hiệu đạt hay không đạt
        Console.WriteLine(what + ": mong doi " + expected + ", thuc te " + actual + " -> " + (ok ? "DAT" : "KHONG DAT"));
    }

    // đo thời gian n lần enqueue rồi n lần dequeue của TwoStackQueue
    static long TimeTwoStack(int n)
    {
        // tạo hàng mới cho mỗi lần đo để các lần đo không ảnh hưởng nhau
        var q = new TwoStackQueue<int>();

        // bắt đầu bấm giờ ngay trước khi xử lý
        var sw = Stopwatch.StartNew();

        // n lần enqueue, mỗi lần Θ(1)
        for (int i = 0; i < n; i++) q.Enqueue(i);

        // n lần dequeue, tổng chi phí đổ là n vì mỗi phần tử đổ đúng một lần
        for (int i = 0; i < n; i++) q.Dequeue();

        // dừng đồng hồ và trả về số mili giây
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    // đo thời gian n lần enqueue rồi n lần dequeue của NaiveTwoStack
    static long TimeNaive(int n)
    {
        // tạo hàng đối chứng mới cho mỗi lần đo
        var q = new NaiveTwoStack<int>();

        // bắt đầu bấm giờ ngay trước khi xử lý
        var sw = Stopwatch.StartNew();

        // n lần enqueue, mỗi lần Θ(n) nên tổng Θ(n²)
        for (int i = 0; i < n; i++) q.Enqueue(i);

        // n lần dequeue, mỗi lần Θ(1)
        for (int i = 0; i < n; i++) q.Dequeue();

        // dừng đồng hồ và trả về số mili giây
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    public static void Main()
    {
        // ca nhỏ bắt buộc: kiểm tra thứ tự FIFO và số lần đổ
        Console.WriteLine("=== Ca nho: enqueue 1,2,3; dequeue; enqueue 4; dequeue 3 lan ===");

        // tạo hàng mới cho ca nhỏ
        var q = new TwoStackQueue<int>();

        // thêm 1, 2, 3 vào ngăn nhận
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);

        // dequeue lần đầu: ngăn lấy rỗng nên đổ 3 phần tử, kết quả phải là 1
        Check("Dequeue", 1, q.Dequeue());

        // thêm 4 vào ngăn nhận trong khi ngăn lấy vẫn còn 2, 3
        q.Enqueue(4);

        // ba lần dequeue tiếp theo phải ra 2, 3, 4: hai lần đầu lấy từ ngăn lấy, lần ba phải đổ thêm 4
        Check("Dequeue", 2, q.Dequeue());
        Check("Dequeue", 3, q.Dequeue());
        Check("Dequeue", 4, q.Dequeue());

        // tổng số lần đổ là 3 (lần đầu) cộng 1 (phần tử 4) bằng 4
        Check("Transfers", 4L, q.Transfers);

        // hàng phải rỗng sau cùng
        Check("Count", 0, q.Count);

        // dequeue trên hàng rỗng phải ném InvalidOperationException
        try { q.Dequeue(); Console.WriteLine("Dequeue hang rong: KHONG DAT (khong nem ngoai le)"); }
        catch (InvalidOperationException) { Console.WriteLine("Dequeue hang rong: nem InvalidOperationException -> DAT"); }

        // ca lớn bắt buộc: một triệu enqueue rồi một triệu dequeue
        Console.WriteLine();
        Console.WriteLine("=== Ca lon: 1.000.000 enqueue roi 1.000.000 dequeue ===");

        // tạo hàng mới cho ca lớn
        var big = new TwoStackQueue<int>();

        // bấm giờ cho ca lớn
        var sw = Stopwatch.StartNew();

        // một triệu lần enqueue
        for (int i = 0; i < 1000000; i++) big.Enqueue(i);

        // cờ ghi nhận thứ tự lấy ra có đúng FIFO hay không
        bool ordered = true;

        // một triệu lần dequeue, so từng phần tử với giá trị mong đợi
        for (int i = 0; i < 1000000; i++)
        {
            // phần tử lấy ra phải đúng bằng i vì thêm vào theo thứ tự tăng dần
            if (big.Dequeue() != i) ordered = false;
        }

        // dừng đồng hồ ngay sau khi xử lý xong
        sw.Stop();

        // thứ tự lấy ra phải đúng FIFO
        Check("Thu tu FIFO", true, ordered);

        // mỗi phần tử đổ đúng một lần nên tổng số lần đổ bằng một triệu
        Check("Transfers", 1000000L, big.Transfers);

        // in thời gian, mong đợi dưới 1 giây
        Console.WriteLine("Thoi gian: " + sw.ElapsedMilliseconds + " ms");

        // bảng 6: so sánh thời gian hai cách cài đặt
        Console.WriteLine();
        Console.WriteLine("=== Bang 6: thoi gian chay (ms) ===");
        Console.WriteLine("n\tNaiveTwoStack\tTwoStackQueue");

        // ba giá trị n theo đề bài
        foreach (int n in new[] { 1000, 5000, 20000 })
        {
            // đo cả hai cách với cùng n rồi in trên một dòng
            Console.WriteLine(n + "\t" + TimeNaive(n) + "\t\t" + TimeTwoStack(n));
        }
    }
}