using System;
using System.Collections.Generic;
using System.Globalization;

// lưu thông tin của một khách để xử lý đúng thứ tự đến
class Customer
{
    // số thứ tự của khách trong danh sách đầu vào
    public int Id;

    // thời điểm khách đến quầy dịch vụ
    public long Arrival;

    // thời gian cần thiết để phục vụ khách
    public long Service;

    // tạo khách với số thứ tự, thời điểm đến và thời gian phục vụ được cung cấp
    public Customer(int id, long arrival, long service)
    {
        Id = id;
        Arrival = arrival;
        Service = service;
    }
}

class Bai2
{
    static void Main2()
    {
        // đọc số lượng khách và số quầy phục vụ
        string[] firstLine = Console.ReadLine().Split();

        // chuyển dữ liệu đầu vào thành số nguyên để dùng trong vòng lặp
        int n = int.Parse(firstLine[0]);
        int k = int.Parse(firstLine[1]);

        // lưu khách theo thứ tự đến để bảo đảm quy tắc FIFO
        Queue<Customer> queue = new Queue<Customer>();

        // đọc dữ liệu từng khách và đưa vào cuối hàng đợi
        for (int i = 1; i <= n; i++)
        {
            // tách thời điểm đến và thời gian phục vụ từ một dòng đầu vào
            string[] input = Console.ReadLine().Split();

            // chuyển thời điểm đến và thời gian phục vụ sang kiểu long để tính toán an toàn
            long arrival = long.Parse(input[0]);
            long service = long.Parse(input[1]);

            // thêm khách vào hàng đợi với số thứ tự bắt đầu từ 1
            queue.Enqueue(new Customer(i, arrival, service));
        }

        // lưu thời điểm mỗi quầy có thể nhận khách tiếp theo, ban đầu tất cả quầy đều rảnh tại thời điểm 0
        long[] freeAt = new long[k];

        // lưu tổng thời gian chờ để tính trung bình mà không bị tràn số int
        long totalWait = 0;

        // lưu thời gian chờ lớn nhất trong tất cả khách
        long maxWait = 0;

        // lưu thời điểm hoàn thành muộn nhất của toàn bộ hệ thống
        long finishTime = 0;

        // lấy từng khách theo thứ tự đến để mô phỏng quá trình phục vụ
        while (queue.Count > 0)
        {
            // lấy khách đầu hàng đợi để bảo đảm khách đến trước được xét trước
            Customer customer = queue.Dequeue();

            // giả sử quầy đầu tiên là quầy rảnh sớm nhất
            int bestCounter = 0;

            // lấy thời điểm rảnh của quầy đầu tiên làm mốc so sánh ban đầu
            long earliestFree = freeAt[0];

            // duyệt các quầy còn lại để tìm quầy rảnh sớm nhất
            for (int j = 1; j < k; j++)
            {
                // chỉ cập nhật khi tìm thấy quầy rảnh sớm hơn, nhờ đó giữ quầy có số thứ tự nhỏ nhất nếu bị hòa
                if (freeAt[j] < earliestFree)
                {
                    earliestFree = freeAt[j];

                    // lưu chỉ số quầy vừa tìm thấy để phục vụ khách hiện tại
                    bestCounter = j;
                }
            }

            // khách chỉ có thể bắt đầu khi cả khách đã đến và quầy đã rảnh
            long startTime = Math.Max(customer.Arrival, earliestFree);

            // tính thời gian chờ bằng thời điểm bắt đầu phục vụ trừ thời điểm đến
            long waitTime = startTime - customer.Arrival;

            // tính thời điểm hoàn thành bằng thời điểm bắt đầu cộng thời gian phục vụ
            long endTime = startTime + customer.Service;

            // cập nhật thời điểm quầy được chọn rảnh trở lại để phục vụ khách tiếp theo
            freeAt[bestCounter] = endTime;

            // cộng thời gian chờ của khách hiện tại vào tổng thời gian chờ
            totalWait += waitTime;

            // cập nhật thời gian chờ lớn nhất nếu khách hiện tại phải chờ lâu hơn
            if (waitTime > maxWait)
            {
                maxWait = waitTime;
            }

            // cập nhật thời điểm hoàn thành muộn nhất của toàn bộ khách đã xử lý
            if (endTime > finishTime)
            {
                finishTime = endTime;
            }

            // in số thứ tự khách, thời điểm đến, số quầy, thời điểm bắt đầu, thời gian chờ và thời điểm hoàn thành
            Console.WriteLine(
                "KH " + customer.Id +
                ": den " + customer.Arrival +
                ", quay " + (bestCounter + 1) +
                ", bat dau " + startTime +
                ", cho " + waitTime +
                ", xong " + endTime
            );
        }

        // tính thời gian chờ trung bình của tất cả khách dưới dạng số thực
        double averageWait = (double)totalWait / n;

        // in thời gian chờ trung bình với hai chữ số thập phân và dấu chấm thập phân
        Console.WriteLine(
            "Cho trung binh: " +
            averageWait.ToString("F2", CultureInfo.InvariantCulture)
        );

        // in thời gian chờ lâu nhất trong số tất cả khách
        Console.WriteLine("Cho lau nhat: " + maxWait);

        // in thời điểm hoàn thành muộn nhất của toàn bộ hệ thống
        Console.WriteLine("Hoan thanh luc: " + finishTime);
    }
}