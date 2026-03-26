#include <iostream>

using namespace std;

long long x_mult;                                   // seed = x_0* для мультипликативного 
const long long c = (long long)INT_MAX / 2 + 3;      
const long long m = (long long)INT_MAX;

double rand_mult() {
    x_mult = (c * x_mult) % m;
    return (double)x_mult / m;
}

long long x_mid;                                    // seed = x_0* для середины квадратов 
long long sq;

double rand_midsq() {
    sq = x_mid * x_mid;                             // возводим в квадрат
    x_mid = (sq / 100) % 10000;                     // берём средние 4 цифры 8-значного числа
    if (x_mid == 0) x_mid = 1234;                   // защита от вырождения
    return (double)x_mid / 10000;
}

int main()
{
    setlocale(LC_ALL, "RUS");

    int seed;
    cout << "Введите начальное зерно (int): ";

    cin >> seed;

    x_mult = seed;
    x_mid = seed % 10000;
    srand(seed);

    int n = 100000;
    double sum, sum2, x, mean, var;


    // мультпликативный коонгруэнтный
    sum = 0;
    sum2 = 0;
    for (int i = 0; i < n; i++) {
        x = rand_mult();
        sum += x;
        sum2 += x * x;
    }
    mean = sum / n;
    var = sum2 / n - mean * mean;
    cout << "Мультпликативный:  М = " << mean << ", D = " << var << endl;

    // середина квадрата
    sum = 0;
    sum2 = 0;
    for (int i = 0; i < n; i++) {
        x = rand_midsq();
        sum += x;
        sum2 += x * x;
    }
    mean = sum / n;
    var = sum2 / n - mean * mean;
    cout << "Середина квадрата: М = " << mean << ", D = " << var << endl;

    // встроенный
    sum = 0;
    sum2 = 0;
    for (int i = 0; i < n; i++) {
        x = (double)rand() / RAND_MAX;
        sum += x;
        sum2 += x * x;
    }
    mean = sum / n;
    var = sum2 / n - mean * mean;
    cout << "Встроенный rand:   М = " << mean << ", D = " << var << endl;
}
