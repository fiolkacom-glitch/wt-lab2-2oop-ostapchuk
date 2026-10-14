namespace wt_lab2_2oop_ostapchuk.Models;

/// <summary>
/// Базовый класс предметной области «Спортивный клуб».
/// Демонстрирует: автосвойства, явный конструктор, публичный метод.
/// </summary>
public class Person
{
    // === Автосвойства ===
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // === Конструктор по умолчанию (для инициализатора объектов) ===
    public Person()
    {
    }

    // === Явный конструктор с параметрами ===
    public Person(int id, string fullName, string phone, string email)
    {
        Id = id;
        FullName = fullName;
        Phone = phone;
        Email = email;
    }

    // === Публичный метод PrintInfo() ===
    public void PrintInfo()
    {
        Console.WriteLine($"  ID:      {Id}");
        Console.WriteLine($"  ФИО:     {FullName}");
        Console.WriteLine($"  Телефон: {Phone}");
        Console.WriteLine($"  Email:   {Email}");
        Console.WriteLine();
    }
}