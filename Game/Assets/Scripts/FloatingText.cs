using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float lifetime = 2f; // Время жизни текста в секундах
    public float floatSpeed = 1f; // Скорость подъема текста

    private void Start()
    {
        Destroy(gameObject, lifetime); // Уничтожаем объект через заданное время
    }

    private void Update()
    {
        // Поднимаем текст вверх
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
    }
}