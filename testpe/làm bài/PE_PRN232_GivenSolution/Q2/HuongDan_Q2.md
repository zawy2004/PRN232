# Hướng dẫn làm bài Q2 (ứng dụng Schedule) — Hướng dẫn chi tiết cho Visual Studio

Mục đích: giải thích chi tiết cách chạy, debug và (nếu cần) hiện thực mock API để kiểm thử trong môi trường Visual Studio.

## Tổng quan nhanh

- `Q2` là một ASP.NET Core MVC project dùng `HttpClient` để gọi API bên ngoài. Controller chính là `Controllers/ScheduleController.cs`.
- Tập trung vào: cấu hình `GivenAPIBaseUrl` trong `appsettings.json`, `Utilities.Initialize(...)`, và cách debug/hiện thực Views (`Views/Schedule/Index.cshtml`, `Details.cshtml`).

## Bước 1 — Mở solution trong Visual Studio

1. Mở Visual Studio (phiên bản 2022 hoặc mới hơn).
2. Chọn `File -> Open -> Project/Solution` và mở file [PE_PRN232_GivenSolution.sln](PE_PRN232_GivenSolution.sln).
3. Trong `Solution Explorer`, tìm project `Q2`.

## Bước 2 — Kiểm tra cấu hình base API

1. Mở [Q2/appsettings.json](appsettings.json#L1).
2. Chỉnh `GivenAPIBaseUrl` để trỏ đến API thật hoặc mock, ví dụ `http://localhost:5100`.

Lưu ý: `Program.cs` đã gọi `Utilities.Initialize(builder.Configuration)` — nên `GivenAPIBaseUrl` phải tồn tại hoặc ứng dụng sẽ ném lỗi.

## Bước 3 — Chạy và debug trong Visual Studio

1. Đặt project `Q2` làm `Startup Project` (click phải vào project `Q2` → `Set as Startup Project`).
2. Mở `Controllers/ScheduleController.cs` và đặt breakpoint ở đầu `Index` và `Details` để quan sát luồng khi truy cập.
3. Chạy ứng dụng bằng nút `IIS Express` hoặc `Kestrel` (F5 để debug, Ctrl+F5 để chạy không debug).
4. Mở trình duyệt tới `https://localhost:{port}/Schedule` (Visual Studio sẽ hiển thị cổng) — khi trang được tải, Visual Studio sẽ dừng ở breakpoint.

Kiểm tra biến tại breakpoint:

- `Utilities.GetAbsoluteUrl(...)` tạo URL đầy đủ.
- `HttpClient` trả về `HttpResponseMessage`; nếu `IsSuccessStatusCode` thì đọc `response.Content.ReadAsStringAsync()` và deserialize sang `ScheduleListItem`/`ScheduleDetail`.

## Bước 4 — Sửa Views nếu cần (Index + Details)

1. Mở `Views/Schedule/Index.cshtml` (nếu file không tồn tại, tạo mới trong `Views/Schedule/`).
2. Tạo form tìm kiếm (GET) gửi `instructor` và `course`:

```html
<form method="get" asp-action="Index">
  <input
    type="text"
    name="instructor"
    value="@ViewBag.Instructor"
    placeholder="Instructor"
  />
  <input
    type="text"
    name="course"
    value="@ViewBag.Course"
    placeholder="Course"
  />
  <button type="submit">Tìm</button>
</form>

<table>
  <thead>
    <tr>
      <th>SectionId</th>
      <th>Course</th>
      <th>Instructor</th>
      <th>Semester</th>
      <th>Room</th>
      <th>Capacity</th>
    </tr>
  </thead>
  <tbody>
    @foreach (var item in Model) {
    <tr>
      <td>
        <a asp-action="Details" asp-route-sectionId="@item.SectionId"
          >@item.SectionId</a
        >
      </td>
      <td>@item.CourseName</td>
      <td>@item.InstructorName</td>
      <td>@item.Semester</td>
      <td>@item.RoomNumber</td>
      <td>@item.MaxCapacity</td>
    </tr>
    }
  </tbody>
</table>
```

3. Mở hoặc tạo `Views/Schedule/Details.cshtml` để hiển thị `ScheduleDetail`:

```html
<h2>Section @Model.SectionId - @Model.CourseName</h2>
<p>Room: @Model.RoomNumber</p>

<h3>Students</h3>
<table>
  <thead>
    <tr>
      <th>Id</th>
      <th>Name</th>
      <th>Grade</th>
    </tr>
  </thead>
  <tbody>
    @foreach(var s in Model.Students) {
    <tr>
      <td>@s.StudentId</td>
      <td>@s.StudentName</td>
      <td>@s.Grade</td>
    </tr>
    }
  </tbody>
</table>
```

## Bước 5 — Nếu không có API thật: tạo Mock API (tùy chọn)

Bạn có thể tạo một project Web API nhỏ trong cùng solution để trả JSON mẫu.

1. Trong Visual Studio: `Add -> New Project` → chọn `ASP.NET Core Web API` → đặt tên `MockApi`.
2. Sửa `Program.cs` của `MockApi` thành minimal API trả dữ liệu mẫu (ví dụ):

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/schedules/search", (string? instructor, string? course) => {
    var list = new[] {
        new { SectionId = 1, InstructorName = "Nguyen A", CourseName = "Math", Semester = "2026A", RoomNumber = "R1", MaxCapacity = 30 }
    };
    return Results.Json(list);
});

app.MapGet("/api/schedules/{sectionId:int}", (int sectionId) => {
    var detail = new {
        SectionId = sectionId,
        CourseName = "Math",
        RoomNumber = "R1",
        Students = new[] { new { StudentId = 101, StudentName = "Tran B", Grade = 8.5 } }
    };
    return Results.Json(detail);
});

app.Run();
```

3. Chạy `MockApi` (F5) — mặc định nó sẽ lắng nghe một cổng khác, ví dụ `http://localhost:5100`.
4. Trong `Q2/appsettings.json`, set `GivenAPIBaseUrl` = `http://localhost:5100`.

Tip: để debug cả hai cùng lúc trong Visual Studio: chọn solution → phải chuột → `Properties` → `Startup Project` → `Multiple startup projects` → chọn `Start` cho `MockApi` và `Q2`.

## Bước 6 — Kiểm thử bằng curl/Postman

- Kiểm thử search:

```bash
curl "http://localhost:5100/api/schedules/search?instructor=&course="
```

- Kiểm thử detail:

```bash
curl "http://localhost:5100/api/schedules/1"
```

Nếu `MockApi` trả JSON hợp lệ, khi chạy `Q2` bạn sẽ thấy dữ liệu xuất lên `Index` và `Details`.

## Lưu ý kỹ thuật và debugging

- `Utilities.GetAbsoluteUrl` sẽ ghép `GivenAPIBaseUrl` và đường dẫn endpoint; kiểm tra xem `GivenAPIBaseUrl` có dấu `/` thừa hay không (hàm đã Trim).
- `JsonSerializerOptions.PropertyNameCaseInsensitive = true` → tên trường JSON có thể khác về hoa thường.
- Nếu API trả lỗi, `ScheduleController` sẽ bẫy ngoại lệ và `Index` trả danh sách rỗng; kiểm tra `response.IsSuccessStatusCode` và `response.StatusCode` khi debug.

## Mẫu JSON trả về

- `GET /api/schedules/search` trả về:

```json
[
  {
    "SectionId": 1,
    "InstructorName": "Nguyen A",
    "CourseName": "Math",
    "Semester": "2026A",
    "RoomNumber": "R1",
    "MaxCapacity": 30
  }
]
```

- `GET /api/schedules/1` trả về:

```json
{
  "SectionId": 1,
  "CourseName": "Math",
  "RoomNumber": "R1",
  "Students": [{ "StudentId": 101, "StudentName": "Tran B", "Grade": 8.5 }]
}
```

## Gợi ý nộp bài (viết báo cáo ngắn)

- Mô tả cách cấu hình `GivenAPIBaseUrl` và cách bạn kiểm thử (API thật hoặc mock).
- Nêu các file bạn đã sửa (ví dụ `Views/Schedule/Index.cshtml`, `Details.cshtml`) và ảnh chụp màn hình.

---

Nếu bạn muốn, tôi có thể tiếp tục và **(a)** tự tạo `MockApi` trong solution, **(b)** cập nhật `Index`/`Details` views theo mẫu phía trên, hoặc **(c)** tạo file JSON mẫu và kịch bản test. Chọn 1 mục bạn muốn tôi làm tiếp.
