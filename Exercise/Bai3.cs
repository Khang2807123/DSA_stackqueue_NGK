using System;
using System.Diagnostics;

// hàng đợi vòng lưu phần tử trong một mảng, dùng front, rear, count để quay vòng chỉ số
public class CircularQueue<T>
{
    // mảng chứa dữ liệu, dung lượng ban đầu là 4 theo yêu cầu đề bài
    private T[] items = new T[4];

    // chỉ số của phần tử đầu hàng, là ô sẽ bị lấy ra ở lần dequeue tiếp theo
    private int front = 0;

    // chỉ số ô trống kế tiếp phía sau phần tử cuối, là ô sẽ được ghi ở lần enqueue tiếp theo
    private int rear = 0;

    // số phần tử đang có, dùng để phân biệt hàng rỗng với hàng đầy khi front == rear
    private int count = 0;

    // số phần tử hiện có, chỉ đọc nên cho ra ngoài bằng thuộc tính
    public int Count => count;

    // dung lượng mảng hiện tại chính là độ dài của mảng đang dùng
    public int Capacity => items.Length;

    // thêm một phần tử vào cuối hàng, tự mở rộng mảng nếu đã đầy
    public void Enqueue(T value)
    {
        // mảng đầy (count bằng dung lượng) thì phải mở rộng trước khi ghi, nếu không sẽ ghi đè phần tử đầu hàng
        if (count == items.Length) Grow();

        // ghi giá trị mới vào ô rear, đây là ô trống ngay sau phần tử cuối
        items[rear] = value;

        // dịch rear sang ô kế tiếp, phép chia lấy dư đưa rear về 0 khi vừa ghi ô cuối mảng
        rear = (rear + 1) % items.Length;

        // tăng số phần tử vì vừa thêm một phần tử
        count++;
    }

    // lấy và xóa phần tử đầu hàng
    public T Dequeue()
    {
        // hàng rỗng thì không có gì để lấy, báo lỗi đúng loại ngoại lệ đề bài yêu cầu
        if (count == 0) throw new InvalidOperationException("hang doi rong");

        // giữ lại giá trị đầu hàng để trả về sau khi đã cập nhật trạng thái
        T value = items[front];

        // đặt ô vừa lấy về default để bộ thu gom rác giải phóng tham chiếu cũ
        items[front] = default!;

        // dịch front sang ô kế tiếp (có quay vòng), không dồn dữ liệu nên chỉ tốn O(1)
        front = (front + 1) % items.Length;

        // giảm số phần tử vì vừa lấy ra một phần tử
        count--;

        // trả về giá trị đầu hàng đã giữ lại ở trên
        return value;
    }

    // xem phần tử đầu hàng mà không xóa
    public T Peek()
    {
        // hàng rỗng thì không có phần tử đầu, báo lỗi như Dequeue
        if (count == 0) throw new InvalidOperationException("hang doi rong");

        // chỉ đọc ô front, không thay đổi front hay count
        return items[front];
    }

    // tạo mảng mới gấp đôi dung lượng và chép các phần tử sang theo đúng thứ tự logic
    private void Grow()
    {
        // mảng mới có dung lượng gấp đôi để việc mở rộng chỉ xảy ra Θ(log n) lần, giúp enqueue O(1) khấu hao
        T[] bigger = new T[items.Length * 2];

        // đoạn thứ nhất đi từ front đến cuối mảng cũ, độ dài bị chặn bởi số phần tử thực có
        int firstPart = Math.Min(count, items.Length - front);

        // chép đoạn thứ nhất vào đầu mảng mới để phần tử đầu hàng nằm ở ô 0
        Array.Copy(items, front, bigger, 0, firstPart);

        // nếu hàng đang quay vòng thì còn đoạn thứ hai nằm ở đầu mảng cũ, chép nối tiếp ngay sau đoạn thứ nhất
        if (firstPart < count) Array.Copy(items, 0, bigger, firstPart, count - firstPart);

        // thay mảng cũ bằng mảng mới đã được sắp lại theo thứ tự FIFO
        items = bigger;

        // phần tử đầu hàng giờ nằm ở ô 0 nên đặt lại front = 0
        front = 0;

        // phần tử cuối nằm ở ô count - 1 nên ô trống kế tiếp là count (nhỏ hơn dung lượng mới nên không cần quay vòng)
        rear = count;
    }
}

class Bai3
{
    // in tiêu đề cho từng ca kiểm thử để ảnh chụp màn hình dễ đọc
    static void Title(string text)
    {
        // dòng trống ngăn cách các ca với nhau
        Console.WriteLine();

        // in tên ca kiểm thử
        Console.WriteLine("=== " + text + " ===");
    }

    // in kết quả so sánh giữa giá trị mong đợi và giá trị thực tế
    static void Check(string what, object expected, object actual)
    {
        // hai giá trị bằng nhau thì ca đạt
        bool ok = Equals(expected, actual);

        // in mong đợi, thực tế và dấu hiệu đạt hay không đạt
        Console.WriteLine(what + ": mong doi " + expected + ", thuc te " + actual + " -> " + (ok ? "DAT" : "KHONG DAT"));
    }

    // lấy hết phần tử ra khỏi hàng và nối thành một chuỗi để in
    static string DrainAll(CircularQueue<string> q, int times)
    {
        // chuỗi kết quả, các phần tử cách nhau bằng khoảng trắng
        string result = "";

        // lấy ra đúng số lần yêu cầu
        for (int i = 0; i < times; i++)
        {
            // dequeue theo thứ tự FIFO rồi nối vào chuỗi kết quả
            result += q.Dequeue() + " ";
        }

        // bỏ khoảng trắng thừa ở cuối trước khi trả về
        return result.Trim();
    }

    public static void Main3()
    {
        // ca 1: hàng mới tạo là hàng rỗng
        Title("Ca 1: hang rong");

        // tạo hàng chuỗi mới
        var q1 = new CircularQueue<string>();

        // hàng mới phải có Count = 0
        Check("Count", 0, q1.Count);

        // hàng mới phải có Capacity = 4
        Check("Capacity", 4, q1.Capacity);

        // dequeue trên hàng rỗng phải ném InvalidOperationException
        try { q1.Dequeue(); Console.WriteLine("Dequeue: KHONG DAT (khong nem ngoai le)"); }
        catch (InvalidOperationException) { Console.WriteLine("Dequeue: nem InvalidOperationException -> DAT"); }

        // peek trên hàng rỗng cũng phải ném InvalidOperationException
        try { q1.Peek(); Console.WriteLine("Peek: KHONG DAT (khong nem ngoai le)"); }
        catch (InvalidOperationException) { Console.WriteLine("Peek: nem InvalidOperationException -> DAT"); }

        // ca 2: thêm đúng 4 phần tử cho vừa đầy mà chưa mở rộng
        Title("Ca 2: vua day");

        // tạo hàng mới cho ca này
        var q2 = new CircularQueue<string>();

        // thêm 4 phần tử A, B, C, D
        foreach (string s in new[] { "A", "B", "C", "D" }) q2.Enqueue(s);

        // đủ 4 phần tử
        Check("Count", 4, q2.Count);

        // dung lượng vẫn là 4 vì mới vừa đầy, chưa cần mở rộng
        Check("Capacity", 4, q2.Capacity);

        // phần tử đầu hàng phải là A
        Check("Peek", "A", q2.Peek());

        // ca 3: quay vòng chỉ số mà không mở rộng
        Title("Ca 3: quay vong, chua mo rong");

        // tạo hàng mới cho ca này
        var q3 = new CircularQueue<string>();

        // thêm A, B, C, D cho đầy mảng
        foreach (string s in new[] { "A", "B", "C", "D" }) q3.Enqueue(s);

        // lấy 2 phần tử đầu ra, được A và B, giải phóng hai ô đầu mảng
        string firstTwo = DrainAll(q3, 2);
        Check("Lay 2 lan", "A B", firstTwo);

        // thêm E, F vào hai ô trống đầu mảng, rear quay vòng về đầu mảng
        q3.Enqueue("E");
        q3.Enqueue("F");

        // lấy hết 4 phần tử còn lại, phải đúng thứ tự FIFO dù dữ liệu nằm vắt qua cuối mảng
        string lastFour = DrainAll(q3, 4);
        Check("Lay 4 lan", "C D E F", lastFour);

        // dung lượng vẫn là 4 vì hàng chưa bao giờ vượt quá 4 phần tử
        Check("Capacity", 4, q3.Capacity);

        // ca 4: mở rộng khi front = 0
        Title("Ca 4: mo rong khi front = 0");

        // tạo hàng số nguyên mới cho ca này
        var q4 = new CircularQueue<int>();

        // thêm 1..5, phần tử thứ 5 làm mảng đầy nên kích hoạt mở rộng lên 8
        for (int i = 1; i <= 5; i++) q4.Enqueue(i);

        // dung lượng phải tăng gấp đôi thành 8
        Check("Capacity", 8, q4.Capacity);

        // lấy 5 phần tử ra và nối thành chuỗi để so sánh thứ tự
        string out4 = "";
        for (int i = 0; i < 5; i++) out4 += q4.Dequeue() + " ";
        Check("Lay 5 lan", "1 2 3 4 5", out4.Trim());

        // ca 5: mở rộng khi front khác 0, đây là ca dễ sai nhất
        Title("Ca 5: mo rong khi front khac 0");

        // tạo hàng mới cho ca này
        var q5 = new CircularQueue<string>();

        // thêm A, B, C, D cho đầy mảng
        foreach (string s in new[] { "A", "B", "C", "D" }) q5.Enqueue(s);

        // lấy A, B ra nên front dịch đến ô 2
        DrainAll(q5, 2);

        // thêm E, F lấp hai ô đầu mảng nên mảng lại đầy với dữ liệu đang vắt qua cuối mảng
        q5.Enqueue("E");
        q5.Enqueue("F");

        // thêm G khi mảng đầy và front = 2: mở rộng phải chép theo thứ tự từ front
        q5.Enqueue("G");

        // dung lượng phải là 8
        Check("Capacity", 8, q5.Capacity);

        // lấy 5 phần tử còn lại, phải ra C D E F G đúng thứ tự
        Check("Lay 5 lan", "C D E F G", DrainAll(q5, 5));

        // ca 6: quay vòng rất nhiều lần, dung lượng không được tăng
        Title("Ca 6: 1.000.000 lan enqueue roi dequeue");

        // tạo hàng mới cho ca này
        var q6 = new CircularQueue<int>();

        // cờ ghi nhận dữ liệu lấy ra có luôn đúng giá trị vừa thêm hay không
        bool allCorrect = true;

        // lặp một triệu lần, mỗi lần thêm một phần tử rồi lấy ngay ra
        for (int i = 0; i < 1000000; i++)
        {
            // thêm giá trị i vào cuối hàng
            q6.Enqueue(i);

            // lấy ra và kiểm tra bằng đúng i vì hàng chỉ có một phần tử
            if (q6.Dequeue() != i) allCorrect = false;
        }

        // toàn bộ lần lấy ra phải đúng giá trị
        Check("Du lieu dung", true, allCorrect);

        // hàng phải rỗng sau cùng
        Check("Count", 0, q6.Count);

        // dung lượng phải vẫn là 4 vì không bao giờ có quá một phần tử
        Check("Capacity", 4, q6.Capacity);

        // ca 7: ca lớn với nhiều lần mở rộng, có đo thời gian
        Title("Ca 7: 100.000 phan tu");

        // tạo hàng mới cho ca này
        var q7 = new CircularQueue<int>();

        // đồng hồ đo thời gian chạy của ca lớn
        var sw = Stopwatch.StartNew();

        // thêm 100000 phần tử, mảng sẽ mở rộng 4 -> 8 -> ... -> 131072
        for (int i = 0; i < 100000; i++) q7.Enqueue(i);

        // cờ ghi nhận thứ tự lấy ra có đúng FIFO hay không
        bool ordered = true;

        // lấy hết ra và so từng phần tử với giá trị mong đợi
        for (int i = 0; i < 100000; i++)
        {
            // phần tử thứ i lấy ra phải đúng bằng i vì thêm vào theo thứ tự tăng dần
            if (q7.Dequeue() != i) ordered = false;
        }

        // dừng đồng hồ ngay sau khi xử lý xong
        sw.Stop();

        // thứ tự lấy ra phải đúng FIFO
        Check("Thu tu FIFO", true, ordered);

        // dung lượng cuối là 131072 (4 nhân 2 mũ 15, nhỏ nhất đủ chứa 100000)
        Check("Capacity", 131072, q7.Capacity);

        // in thời gian để ghi vào bảng ca lớn
        Console.WriteLine("Thoi gian: " + sw.ElapsedMilliseconds + " ms");
    }
}