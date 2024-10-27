# Используем официальный образ Python
FROM python:3.9-slim

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /app

# Копируем файлы requirements.txt в контейнер
COPY requirements.txt .

# Устанавливаем зависимости
RUN pip install -r requirements.txt

# Копируем все файлы вашего приложения в контейнер
COPY . .

# Указываем порт, который будет использоваться
EXPOSE 5000

# Запускаем приложение
CMD ["python", "app.py"]
