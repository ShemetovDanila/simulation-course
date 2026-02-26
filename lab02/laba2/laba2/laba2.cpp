#include <iostream>
#include <iomanip>
#include <vector>
#include <cmath>
#include <string>

using namespace std;

double solve(double L, double tau, double h, double T_left, double T_right,
    double T_init, double T_end, double rho, double c, double lambda) {

    int N = static_cast<int>(round(L / h));

    if (N < 2) {
        N = 2;
    }

    int time_steps_total = static_cast<int>(round(T_end / tau));
    double dt = T_end / time_steps_total;

    vector<double> T_prev(N + 1, T_init);
    vector<double> T_new(N + 1);
    vector<double> alpha(N + 1);
    vector<double> beta(N + 1);

    double lambda_h2 = lambda / (h * h);
    double rho_c_tau = (rho * c) / dt;
    double A = lambda_h2;
    double C_coef = lambda_h2;
    double B = 2.0 * lambda_h2 + rho_c_tau;

    T_prev[0] = T_left;
    T_prev[N] = T_right;

    for (int step = 0; step < time_steps_total; step++) {
        vector<double> F(N + 1);
        for (int i = 0; i <= N; i++) {
            F[i] = -rho_c_tau * T_prev[i];
        }

        alpha[1] = A / B;
        beta[1] = (C_coef * T_left - F[1]) / B;

        for (int i = 2; i < N; i++) {
            alpha[i] = A / (B - C_coef * alpha[i - 1]);
            beta[i] = (C_coef * beta[i - 1] - F[i]) / (B - C_coef * alpha[i - 1]);
        }

        T_new[N] = T_right;
        for (int i = N - 1; i >= 1; i--) {
            T_new[i] = alpha[i] * T_new[i + 1] + beta[i];
        }
        T_new[0] = T_left;

        T_prev = T_new;
    }

    return T_prev[N / 2];
}

int main() {
    setlocale(LC_ALL, "Russian");

    cout << "=== СПРАВОЧНИК МАТЕРИАЛОВ ===" << endl;
    cout << "1. Сталь      (rho=7800, c=460, lambda=46)" << endl;
    cout << "2. Медь       (rho=8960, c=385, lambda=401)" << endl;
    cout << "3. Чугун      (rho=7200, c=540, lambda=52)" << endl;
    cout << "4. Золото     (rho=19300, c=129, lambda=317)" << endl;
    cout << "5. Вольфрам   (rho=19250, c=132, lambda=173)" << endl;
    cout << "6. Алюминий   (rho=2700, c=897, lambda=237)" << endl;
    cout << "0. Ввести свои данные" << endl << endl;

    int choice;
    double rho, c, lambda;
    cout << "Выберите материал (0-6): ";
    cin >> choice;

    switch (choice) {
    case 1: rho = 7800; c = 460; lambda = 46; break;
    case 2: rho = 8960; c = 385; lambda = 401; break;
    case 3: rho = 7200; c = 540; lambda = 52; break;
    case 4: rho = 19300; c = 129; lambda = 317; break;
    case 5: rho = 19250; c = 132; lambda = 173; break;
    case 6: rho = 2700; c = 897; lambda = 237; break;
    default:
        cout << "Плотность rho (кг/м3): "; cin >> rho;
        cout << "Теплоемкость c (Дж/кг·C): "; cin >> c;
        cout << "Теплопроводность lambda (Вт/м·C): "; cin >> lambda;
        break;
    }

    double L, T_left, T_right, T_init, T_end;
    cout << "\nДлина пластины L (м): "; cin >> L;
    cout << "Температура слева T_left (C): "; cin >> T_left;
    cout << "Температура справа T_right (C): "; cin >> T_right;
    cout << "Начальная температура T_init (C): "; cin >> T_init;
    cout << "Время моделирования (с): "; cin >> T_end;

    vector<double> time_steps = { 0.1, 0.01, 0.001, 0.0001 };
    vector<double> space_steps = { 0.1, 0.01, 0.001, 0.0001 };

    cout << fixed << setprecision(4);
    cout << "\n=== ТАБЛИЦА: ТЕМПЕРАТУРА В ЦЕНТРЕ ПЛАСТИНЫ (C) ПОСЛЕ " << T_end << "c ===" << endl;
    cout << "Столбец - шаг по пространству h (м); Строка - шаг по времени tau (с)" << endl << endl;

    int col_width = 14;

    cout << "+------------+";
    for (size_t i = 0; i < time_steps.size(); i++)
        cout << string(col_width, '-') << "+";
    cout << endl;

    cout << "| " << left << setw(10) << "h \\ tau" << " |";
    for (double tau : time_steps)
        cout << " " << right << setw(col_width - 1) << tau << "|";
    cout << endl;

    cout << "+------------+";
    for (size_t i = 0; i < time_steps.size(); i++)
        cout << string(col_width, '-') << "+";
    cout << endl;

    for (size_t hi = 0; hi < space_steps.size(); hi++) {
        double h = space_steps[hi];
        cout << "| " << left << setw(10) << h << " |";

        for (size_t ti = 0; ti < time_steps.size(); ti++) {
            double tau = time_steps[ti];
            double result = solve(L, tau, h, T_left, T_right, T_init, T_end, rho, c, lambda);
            
            cout << " " << right << setw(col_width - 1) << result << "|";
            
            cout.flush();
        }
        cout << endl;

        cout << "+------------+";
        for (size_t i = 0; i < time_steps.size(); i++)
            cout << string(col_width, '-') << "+";
        cout << endl;
    }

    return 0;
}