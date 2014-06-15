//
//  IBeaconReceiver.m
//  ble_plugin
//
//  Created by Michael Hoffstaedter on 10.02.14.
//  Copyright (c) 2014 Michael Hoffstaedter. All rights reserved.
//

#import "IBeaconReceiver.h"


@interface IBeaconReceiver() <CBCentralManagerDelegate,CLLocationManagerDelegate,
UIAlertViewDelegate>

@property (strong,nonatomic) CLBeaconRegion *beaconRegion;
@property (strong,nonatomic) NSMutableArray *beaconRegions;
@property (strong,nonatomic) CLLocationManager *locationManager;


//@property NSMutableDictionary *beacons;
//@property NSMutableDictionary *rangedRegions;
@property bool should_log_debug;
@end

@implementation IBeaconReceiver



- (void) InitWithUUID:(NSUUID *)uuid
  andRegionIdentifier:(NSString *)identifier
               andLog:(BOOL)log {
    
//    self.beacons = [[NSMutableDictionary alloc] init];
//    
//    // Populate the regions we will range once.
//    self.rangedRegions = [[NSMutableDictionary alloc] init];
    
    
    self.locationManager = [[CLLocationManager alloc] init];
    self.locationManager.delegate = self;
    
    [self startScanWithUUID:uuid andRegionIdentifier:identifier];
    
    self.should_log_debug = log;
    if (_should_log_debug)
        NSLog(@"Started location manager");
}

- (void) startScanWithUUID:(NSUUID *)uuid
       andRegionIdentifier:(NSString *)identifier {
    
    CLBeaconRegion *tempRegion = [[CLBeaconRegion alloc] initWithProximityUUID:uuid
                                                                    identifier:identifier];
    if (self.beaconRegions == nil)
        self.beaconRegions = [[NSMutableArray alloc] initWithObjects:tempRegion, nil];
    else
        [self.beaconRegions addObject:tempRegion];
    
    
//    self.rangedRegions[tempRegion] = [NSArray array];
    
    [self.locationManager startMonitoringForRegion:tempRegion];
    [self.locationManager startRangingBeaconsInRegion:tempRegion];
    if (_should_log_debug) {
        NSLog(@"Monitoring and Ranging turned on for region: %@.", self.beaconRegion);
    }
}


- (void) stopScan {
    for (CLBeaconRegion *reg in self.beaconRegions)
    {
        [self.locationManager stopMonitoringForRegion:reg];
        [self.locationManager stopRangingBeaconsInRegion:reg];
    }
    [self.beaconRegions removeAllObjects];
    
}
#pragma mark CLLocationManagerDelegate methods

- (void) locationManager:(CLLocationManager *)manager
rangingBeaconsDidFailForRegion:(CLBeaconRegion *)region
               withError:(NSError *)error
{
    NSLog(@"rangingBeaconsDidFailForRegion %@.", error);
}

- (void)locationManager:(CLLocationManager *)manager
didChangeAuthorizationStatus:(CLAuthorizationStatus)status
{
    if (![CLLocationManager locationServicesEnabled]) {
        NSLog(@"Couldn't turn on Receiver: Location services are not enabled.");
        return;
    }
    
    if ([CLLocationManager authorizationStatus] != kCLAuthorizationStatusAuthorized) {
        
        NSLog(@"Couldn't turn on Receiver: Location services not authorised.");
        return;
    }
    
}

- (void) locationManager:(CLLocationManager *)manager
didStartMonitoringForRegion:(CLRegion *)region {
    NSLog(@"Monitor started, request state for region:%@.",region);
    [self.locationManager requestStateForRegion:region];
}

- (void) locationManager:(CLLocationManager *)manager
       didDetermineState:(CLRegionState)state
               forRegion:(CLRegion *)region {
    NSString *stateString = nil;
    switch (state) {
        case CLRegionStateInside:
            stateString = @"inside";
            break;
        case CLRegionStateOutside:
            stateString = @"outside";
            break;
        case CLRegionStateUnknown:
            stateString = @"unknown";
            break;
    }
    if (_should_log_debug) {
        NSLog(@"State changed to %@ for region %@.", stateString, region);
    }
    
}

- (void) locationManager:(CLLocationManager *)manager
          didEnterRegion:(CLRegion *)region {
    if (_should_log_debug)
        NSLog(@"Entered region: %@", region);
    
    //    [self sendLocalNotificationForBeaconRegion:(CLBeaconRegion *)region];
    
}

- (void) locationManager:(CLLocationManager *)manager
           didExitRegion:(CLRegion *)region {
    if (_should_log_debug)
        NSLog(@"Exited region: %@", region);
}

extern double kalmanfilter_update(double measurement);

- (void) locationManager:(CLLocationManager *)manager
         didRangeBeacons:(NSArray *)beacons
                inRegion:(CLBeaconRegion *)region
{
    
//    NSLog(@"-------\n%lu beacons found\n",(unsigned long)beacons.count);
//    for (int i = 0; i<beacons.count; i++) {
//        NSLog(@"\t\t%@\n",beacons[i]);
//    }
    
    //    self.rangedRegions[region] = beacons;
    //    [self.beacons removeAllObjects];
    //
    //    NSMutableArray *allBeacons = [NSMutableArray array];
    //
    //    for (NSArray *regionResult in [self.rangedRegions allValues])
    //    {
    //        [allBeacons addObjectsFromArray:regionResult];
    //    }
    //
    //    for (NSNumber *range in @[@(CLProximityUnknown), @(CLProximityImmediate), @(CLProximityNear), @(CLProximityFar)])
    //    {
    //        NSArray *proximityBeacons = [allBeacons filteredArrayUsingPredicate:[NSPredicate predicateWithFormat:@"proximity = %d", [range intValue]]];
    //        if([proximityBeacons count])
    //        {
    //            self.beacons[range] = proximityBeacons;
    //        }
    //    }
    //    NSArray *sorted_all_beacons = [self.beacons allValues];
    
    
    
    NSMutableString *data = [NSMutableString stringWithString:@""];
    for (CLBeacon *beacon in beacons) {
        int proximity = 0;
        if (beacon.proximity == CLProximityFar) {
            proximity = 1;
        } else if (beacon.proximity == CLProximityNear) {
            proximity = 2;
        } else if (beacon.proximity == CLProximityImmediate) {
            proximity = 3;
        }
        
        double facc = beacon.accuracy;//kalmanfilter_update(beacon.accuracy);
        [data appendFormat:@"%@,%d,%d,%d,%ld,%f;",beacon.proximityUUID.UUIDString,beacon.major.intValue,beacon.minor.intValue,proximity,(long)beacon.rssi,facc];
        
    }
    //if (_should_log_debug)
    //    NSLog(@"IOS: Sending %@",data);
    UnitySendMessage("IBeaconReceiver","RangeBeacons",[[NSString stringWithString:data] cStringUsingEncoding:NSUTF8StringEncoding]);
}

#pragma avilable checks by jlk

-(void)centralManagerDidUpdateState:(CBCentralManager *)central{
    bool is_errstate = false;
    NSString* err_message = @"";
    switch (central.state) {
            
        case CBCentralManagerStatePoweredOn:
            NSLog(@"Bluetooth available");
            break;
        case CBCentralManagerStateUnsupported:
            is_errstate = YES;
            err_message = @"The platform does not support Bluetooth low energy.";
            break;
        default:
            is_errstate = YES;
            err_message = [NSString stringWithFormat: @"Bluetooth not available:%d, please open and authorize to use.",cbCentralMgr.state];
            break;
    }
    if (is_errstate) {
        [self showMessages:err_message
                 withTitle:@"APP Will Exit"
             isCriticalErr:NO];
    }
}

bool should_exit_after_tips = false;
- (void) showMessages:(NSString*)message
{
    [self showMessages:message
             withTitle:@"Message"
         isCriticalErr:NO];
}
- (void) showMessages:(NSString*)message
            withTitle:(NSString*) title
        isCriticalErr:(bool) isCriticalErr
{
    NSLog(@"%@",message);
    should_exit_after_tips = isCriticalErr;
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Messages"
                                                    message:message
                                                   delegate: self
                                          cancelButtonTitle:nil
                                          otherButtonTitles:@"OK",nil];
    
    [alert show];
    [alert release];
    
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{
    if (should_exit_after_tips) {
        exit(0);
        NSLog(@"Exiting...");
    }
}

CBCentralManager *cbCentralMgr;
- (BOOL) checkDeviceAvilable
{
    NSLog(@"Check Bluetooth  ...");
    
    cbCentralMgr = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
    
    NSLog(@"Checking Location Service ...");
    if (![CLLocationManager locationServicesEnabled]) {
        [self showMessages:@" Location services are not enabled."];
    }
    if ([CLLocationManager authorizationStatus] != kCLAuthorizationStatusAuthorized) {
        [self showMessages:@"Location services not authorised."];
    }
    
    NSLog(@"Checking Ranging require ...");
    
    if (![CLLocationManager isRangingAvailable]) {
        [self showMessages:@" Ranging is not available."];
        return NO;
    }
    
    if (self.locationManager.rangedRegions.count > 0) {
        [self showMessages:@"Ranging already on."];
        return NO;
    }
    NSLog(@"Checking Monitor require ...");
    
    if (![CLLocationManager isMonitoringAvailableForClass:[CLBeaconRegion class]]) {
        [self showMessages:@" Region monitoring is not available for CLBeaconRegion class."];
        return NO;
    }
    return YES;
}


@end

IBeaconReceiver *currentReceiver;

bool InitReceiver(char * uuid, char * regionIdentifier, bool simulateRegionEnter, bool shouldLog) {
    
    
    
    if (currentReceiver == nil) {
        currentReceiver = [IBeaconReceiver alloc];
        BOOL device_avilable = [currentReceiver checkDeviceAvilable];
        if (!device_avilable) {
            currentReceiver = nil;
            return NO;
        }
        [currentReceiver InitWithUUID:[[NSUUID alloc] initWithUUIDString:[NSString stringWithUTF8String:uuid]]
                  andRegionIdentifier:[NSString stringWithUTF8String:regionIdentifier]
                               andLog:shouldLog];
    } else {
        [currentReceiver startScanWithUUID:[[NSUUID alloc]initWithUUIDString:[NSString stringWithUTF8String:uuid]]
                       andRegionIdentifier:[NSString stringWithUTF8String:regionIdentifier]];
    }
    return YES;
}

void StopIOSScan() {
    [currentReceiver stopScan];
}


/*
 
 //NSArray *filteredBeacons = [self filteredBeacons:beacons];
 //    if (should_log_debug)
 //    {
 //        if (filteredBeacons.count == 0) {
 //            NSLog(@"No beacons found nearby.");
 //        } else {
 //            NSLog(@"Found %lu %@.", (unsigned long)[filteredBeacons count],
 //                [filteredBeacons count] > 1 ? @"beacons" : @"beacon");
 //        }
 //    }
 //- (NSArray *)filteredBeacons:(NSArray *)beacons
 //{
 //    // Filters duplicate beacons out; this may happen temporarily if the originating device changes its Bluetooth id
 //    NSMutableArray *mutableBeacons = [beacons mutableCopy];
 //
 //    NSMutableSet *lookup = [[NSMutableSet alloc] init];
 //    for (int index = 0; index < [beacons count]; index++) {
 //        CLBeacon *curr = [beacons objectAtIndex:index];
 //        NSString *identifier = [NSString stringWithFormat:@"%@/%@", curr.major, curr.minor];
 //
 //        // this is very fast constant time lookup in a hash table
 //        if ([lookup containsObject:identifier]) {
 //            [mutableBeacons removeObjectAtIndex:index];
 //        } else {
 //            [lookup addObject:identifier];
 //        }
 //    }
 //
 //    return [mutableBeacons copy];
 //}
 
 
 - (void) locationManager:(CLLocationManager *)manager didEnterRegion:(CLRegion *)region {
 if (should_log_debug)
 NSLog(@"Entered region: %@", region);
 
 [self sendLocalNotificationForBeaconRegion:(CLBeaconRegion *)region];
 
 
 //
 //    NSPredicate *regionPredicate = [NSPredicate predicateWithFormat:@"identifier = %@",region.identifier];
 //    NSArray *tempArray = [[self.beaconRegions copy] filteredArrayUsingPredicate:regionPredicate];
 //    for (CLBeaconRegion *cl in tempArray)
 //        [self.locationManager startRangingBeaconsInRegion:cl];
 
 
 }
 
 - (void) locationManager:(CLLocationManager *)manager didExitRegion:(CLRegion *)region {
 if (should_log_debug)
 NSLog(@"Exited region: %@", region);
 
 //    NSPredicate *regionPredicate = [NSPredicate predicateWithFormat:@"identifier = %@",region.identifier];
 //    NSArray *tempArray = [[self.beaconRegions copy] filteredArrayUsingPredicate:regionPredicate];
 //    for (CLBeaconRegion *cl in tempArray)
 //        [self.locationManager stopRangingBeaconsInRegion:cl];
 }
 
 
 */