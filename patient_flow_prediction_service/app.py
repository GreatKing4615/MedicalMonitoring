from flask import Flask, request, jsonify
import pandas as pd
import numpy as np
from statsmodels.tsa.arima.model import ARIMA
from datetime import datetime, timedelta
import json
import logging
logging.basicConfig(level=logging.DEBUG)

app = Flask(__name__)

# Маршрут для проверки работоспособности сервиса
@app.route('/', methods=['GET'])
def index():
    return "Patient Flow Prediction Service is running."

# Маршрут для прогнозирования
@app.route('/predict', methods=['POST'])
def predict():
    try:
        data = request.get_json()

        # Получаем данные из запроса
        historical_data = data['historical_data']
        horizon = data.get('horizon', 7)
        max_capacity = data.get('max_capacity', 100)

        # Проверка на деление на ноль
        if max_capacity == 0:
            return jsonify({'error': 'max_capacity должен быть больше нуля.'}), 400

        # Преобразуем данные в DataFrame
        df = pd.DataFrame(historical_data)
        df['Date'] = pd.to_datetime(df['Date'])
        df.set_index('Date', inplace=True)

        # Обучаем модель ARIMA
        model = ARIMA(df['PatientCount'], order=(1,1,1))
        model_fit = model.fit()

        # Прогнозируем
        forecast = model_fit.forecast(steps=horizon)

        # Определяем даты прогноза
        last_date = df.index[-1]
        forecast_dates = [last_date + timedelta(days=i) for i in range(1, horizon+1)]

        # Формируем результат
        results = []
        for date, count in zip(forecast_dates, forecast):
            predicted_count = int(round(count))
            load_percentage = (float(predicted_count) / float(max_capacity)) * 100.0
            load_percentage = load_percentage
            logging.debug(f"Date: {date}, Predicted Count: {predicted_count}, Max Capacity: {max_capacity}, Load Percentage: {load_percentage}")

            results.append({
                'Date': date.strftime('%Y-%m-%d'),
                'PredictedPatientCount': predicted_count,
                'LoadPercentage': load_percentage,
                'IsOverloaded': load_percentage > 100.0
            })

        return jsonify(results)
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
