//
//  IBeaconReceiver.h
//  ble_plugin
//
//  Created by Michael Hoffstaedter on 10.02.14.
//  Copyright (c) 2014 Michael Hoffstaedter. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#import <CoreLocation/CoreLocation.h>

extern void UnitySendMessage(const char *, const char *, const char *);

@interface IBeaconReceiver : NSObject 
@end
