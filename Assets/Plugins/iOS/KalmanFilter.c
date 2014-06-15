//
//  KalmanFilter.c
//  AirLocate
//
//  Created by BradJiao on 14-6-15.
//  Copyright (c) 2014年 Apple. All rights reserved.
//

#include <stdio.h>

static double Q = 0.00001;
static double R = 0.01;
static double P = 1, X = 0, K;

static void measurementUpdate() {
    K = (P + Q) / (P + Q + R);
    P = R * (P + Q) / (R + P + Q);
}

double kalmanfilter_update(double measurement) {
    measurementUpdate();
    double result = X + (measurement - X) * K;
    X = result;
    return result;
}