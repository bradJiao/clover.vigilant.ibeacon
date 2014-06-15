//
//  ExtendMinMax.c
//  AirLocate
//
//  Created by BradJiao on 14-6-14.
//  Copyright (c) 2014年 Apple. All rights reserved.
//

#include <stdio.h>
#include <stdlib.h>

typedef struct {
    double x;
    double y;
} Point;


typedef struct {
    double range;
    Point pos;
    
}ArchorNode;

typedef struct{
    Point           pos;
    ArchorNode*     archor_list;
    int             archor_num;
    
} Node;

typedef int bool;
#define false   0
#define true    1
#define null    0

typedef struct
{
    double Xmin, Xmax, Ymin, Ymax;
}BoundingBox;

void initBoundingBox(BoundingBox* b, Point center, double deviation)
{
    b->Xmin = center.x - deviation;
    b->Xmax = center.x + deviation;
    b->Ymin = center.y - deviation;
    b->Ymax = center.y + deviation;
}


double Max(double a, double b){return a>b?a:b;}
double Min(double a, double b){return a<b?a:b;}

void MinMaxCalc(ArchorNode* Anchors, int archornum,Point* center);
bool BelongsToAllBoxes(double x1, double y1, double r1, double x2, double y2, double r2);


static int MaxSearchIntersectedCount  = 10;
/// <summary>
/// Calculates the position
/// </summary>
/// <param name="BlindNode">The BlindNode to be positioned</param>
/// <returns>The position of the blind node</returns>
bool CalculatePosition(Node* BlindNode, Point* center)
{
    bool AllBoxesIntersected = false;
    
    int search_count  = 0;
    
    while (!AllBoxesIntersected && search_count < MaxSearchIntersectedCount)
    {
        AllBoxesIntersected = true;
        
        for (int i = 0; i < BlindNode->archor_num && AllBoxesIntersected; i++)
        {
            
            for (int j = 0; j < BlindNode->archor_num && AllBoxesIntersected; j++)
            {
                
                if (!BelongsToAllBoxes(BlindNode->archor_list[i].pos.x,
                                       BlindNode->archor_list[i].pos.y,
                                       BlindNode->archor_list[i].range,
                                       BlindNode->archor_list[j].pos.x,
                                       BlindNode->archor_list[j].pos.y,
                                       BlindNode->archor_list[j].range))
                {
                    AllBoxesIntersected = false;
                }
            }
        }
        if (!AllBoxesIntersected)
        {
            for (int i=0; i< BlindNode->archor_num; i++) {
                BlindNode->archor_list[i].range *=1.1;
            }
            
        }
        search_count ++;
    }
    
    
    if (BlindNode->archor_num >= 3 && search_count < MaxSearchIntersectedCount)
    {
        MinMaxCalc(BlindNode->archor_list,BlindNode->archor_num,center);
        return true;
    }
    else
    {
        return false;
    }
    
    
}

/// <summary>
/// Calculates the intersection points between two circles
/// </summary>
/// <param name="Anchors">List of anchor nodes</param>
/// <param name="filterMethod">The filter to use on the RSS values</param>
/// <returns>The center of the box in common</returns>
void MinMaxCalc(ArchorNode* Anchors, int archornum,Point* center)
{
    BoundingBox BnBox, AnBox;
    double distance = Anchors[0].range;
    
    center->x = Anchors[0].pos.x;
    center->y = Anchors[0].pos.y;
    
    initBoundingBox(&AnBox, *center, distance);
    
    BnBox = AnBox;
    
    for (int i = 1; i < archornum; i++)
    {
        
        distance = Anchors[i].range;
        
        center->x = Anchors[i].pos.x;
        center->y = Anchors[i].pos.y;
        
        initBoundingBox(&AnBox, *center, distance);
        
        BnBox.Xmin = Max(BnBox.Xmin, AnBox.Xmin);
        BnBox.Xmax = Min(BnBox.Xmax, AnBox.Xmax);
        BnBox.Ymin = Max(BnBox.Ymin, AnBox.Ymin);
        BnBox.Ymax = Min(BnBox.Ymax, AnBox.Ymax);
    }
    center->x = (BnBox.Xmin + BnBox.Xmax) / 2;
    center->y = (BnBox.Ymin + BnBox.Ymax) / 2;
}
/// <summary>
/// Checks if the boxes are out of range
/// </summary>
/// <param name="x1">x coordinate of the first cicrcle</param>
/// <param name="y1">y coordinate of the first cicrcle</param>
/// <param name="r1">the radius of the first circle</param>
/// <param name="x2">x coordinate of the second cicrcle</param>
/// <param name="y2">y coordinate of the second cicrcle</param>
/// <param name="r2">the radius of the second circle</param>
/// <returns>Returns true if the boxes cut</returns>
bool BelongsToAllBoxes(double x1, double y1, double r1, double x2, double y2, double r2)
{
    Point a1, a2;
    a1.x = x1;
    a1.y = y1;
    a2.x = x2;
    a2.y = y2;
    
    BoundingBox bn1, bn2;
    initBoundingBox(&bn1, a1, r1);
    initBoundingBox(&bn2, a2, r2);
    
    if(bn1.Xmax < bn2.Xmin || bn1.Xmin > bn2.Xmax || bn1.Ymax < bn2.Ymin || bn1.Ymin > bn2.Ymax)
        return false;
    else
        return true;
}


//////////test

bool calcPos(double data[], int archorNum, double* outx,double* outy)
{
    ArchorNode * archorlist = calloc(archorNum,sizeof(ArchorNode));
    if (archorlist == null) {
        return false;
    }
    for (int i=0; i<archorNum; i++) {
        archorlist[i].pos.x = data[i*3+0];
        archorlist[i].pos.y = data[i*3+1];
        archorlist[i].range = data[i*3+2];
    }
    
    Node node;
    node.archor_list = archorlist;
    node.archor_num = archorNum;
    Point center;
    
    
    
    bool ret = CalculatePosition(&node, &center);
    if (ret) {
        *outx = center.x;
        *outy = center.y;
    }
    
    free(archorlist);
    
    return  ret;
}

int test_main()
{
    //    Node node;
    //    ArchorNode archorlist[3];
    //    node.archor_list =archorlist;
    //    node.archor_num = 3;
    //    Point center;
    //
    //    archorlist[0].pos.x = 5;
    //    archorlist[0].pos.y = 0;
    //    archorlist[0].range = 4;
    //
    //    archorlist[1].pos.x = 0;
    //    archorlist[1].pos.y = 5;
    //    archorlist[1].range = 4;
    //
    //    archorlist[2].pos.x = 10;
    //    archorlist[2].pos.y = 5;
    //    archorlist[2].range = 4;
    //
    //    CalculatePosition(&node, &center);
    
    double data[18];
    data[0] = 5;
    data[1] = 0;
    data[2] = 4;
    
    data[3] = 0;
    data[4] = 5;
    data[5] = 4;
    
    data[6] = 10;
    data[7] = 5;
    data[8] = 4;
    
    double ox,oy;
    
    calcPos(data, 3, &ox, &oy);
    
    return 0;
}
