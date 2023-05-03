# user-service dasboards in grafana

## 1_latency_app_user
Latency (response time) методов Create, Update, Get. Настроен алертинг: если среднее время ответа будет больше 100мс в течение 5 минут, придет уведомление на почту.

## 2_rpc_app_user
RPS (кол-во запросов в секунду) методов Create, Update, Get. 

## 3_error_app_user
500 Error (кол-во ошибок) методов Create, Update, Get. Настроен алертинг: если среднее кол-во ошибок будет больше 5 в течение 5 минут, придет уведомление на почту.

## 4_ngnix_ingress_controller
Latency, RPS, 500 Error по ngnix_ingress_controller. Настроены алертинги.

## 5_cpu_memory
Потребление подами приложения памяти и CPU

## 6_database_stat
Метрики по БД. Добавил графики из готовых дашбордов: https://grafana.com/grafana/dashboards/9628-postgresql-database/ и https://grafana.com/grafana/dashboards/455-postgres-overview/

## dashboard.json
Шаблон дашборда.