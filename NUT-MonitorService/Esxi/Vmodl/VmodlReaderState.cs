using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi.Vmodl
{
    internal enum VmodlReaderState
    {
        //USE                PUSH BACK: (100)
        //Expecting:
        // (     - Object type name on next            ->  (1)
        // '     - ref typename                        ->  (2)
        // "     - a string value                      ->  (8)
        // <     - UnsetValue                          ->  (10)
        // digit - integer number, pos--               ->  (9)
        // (space), \r, \n, \t  - ignore               ->  (1)

        //USE                PUSH BACK: (100)
        // any                - object start, pos--    ->  (15)
        Initial = 0,

        //Expecting:
        //  letterOrDigits    - type name              ->  (1)
        //  .                 - type name              ->  (1)
        //  )                 - type name end          ->  (3) //(12)
        TypeNameForObject_1 = 1,

        //Expecting:
        //  letterOrDigits      - type name            ->  (2)
        //  :                   - go to RefId          ->  (11)
        //  '                   - inavlid character    ->  (2)
        TypeNameForRef_2 = 2,

        //Expecting:
        //  {                   - parse body           ->  (4)
        //  [                   - parse array. PB(16)  ->  (15)          //TODO !!!!
        //  n                   - parse null           ->  (14)
        // (space), \r, \n, \t  - ignore               ->  (3)
        WaitObjectBody_3 = 3,

        //Expecting:
        //  letter               - property name       ->  (5)
        //  }                    - endBody. go back    ->  (B)
        //  (space), \r, \n, \t  - ignore              ->  (4)
        ObjectBody_4 = 4,

        //Expecting:
        //  letterOrDigits    - property name          ->  (5)
        //  =                 - wait prop value        ->  (6)
        // (space), \r, \n, \t  - ignore               ->  (5)
        PropertyName_5 = 5,

        //USE           PUSH BACK: (7)
        //Expecting:
        //  digits/-/+          - number value         ->  (9)
        //  "                   - string value         ->  (8)
        //  '                   - ref typename         ->  (2)
        //  (                   - object value         ->  (1)
        //  leading(space), \r, \n, \t  - ignor        ->  (6)

        //USE           PUSH BACK: (7)
        // any                - object start, pos--    ->  (15)
        PropertyValue_6 = 6,



        //Expecting:
        //  ,                    - object body         ->  (4)
        //  non-space except ,   - object body, pos--  ->  (4)
        //  (space), \r, \n, \t  - ignore              ->  (7)
        PropertyEnd_7 = 7,

        //Expecting:
        // " (except \")     - Go back                 ->  (B) 
        //  any another char - populate buffer         ->  (8)
        StringValue_8 = 8,

        //Expecting:
        //  digits          - value                    ->  (9)
        // .                - decimal separator        ->  (9)
        // non-digit        - Parse value and go back  ->  (B)  For \, do      pos--
        NumberValue_9 = 9,

        //Expecting:
        //  u,n,s,e,t       - value                    ->  (10)
        // >                - go back                  ->  (10)
        UnsetValue_10 = 10,

        //Expecting:
        //  letterOrDigits      - RefId                ->  (11)
        //  '                   - go back              ->  (B)
        RefId_11 = 11,

        //Expecting:
        //  {                   - wait open body       ->  (4)
        // (space), \r, \n, \t  - ignore               ->  (10)
        //TypeNameExpectingEnd_12 = 12,

        //Expecting:
        // u,l             - letters to put in buffer  ->  (14)
        // (space), \r,  
        //   \n, \t, \,    - check buffer. push back   ->  (B)
        NullValue_14 = 14,


        // (     - Object type name on next            ->  (1)
        // '     - ref typename                        ->  (2)
        // "     - a string value                      ->  (8)
        // <     - UnsetValue                          ->  (10)
        // t     - true parse                          ->  (18)
        // f     - false parse                         ->  (19)
        // digit - integer number, pos--               ->  (9)
        // (space), \r, \n, \t  - ignore               ->  (1)
        ValueStart_15 = 15,


        //Expecting:
        //  ,                    - next item           ->  (17)
        //  ]                    - go back             ->  (B)
        //  (space), \r, \n, \t  - ignore              ->  (16)
        NextArrayItem_16 = 16,


        //Expecting:
        //  ]                    - go back             ->  (B)
        //  (space), \r, \n, \t  - ignore              ->  (16)
        //  the rest             - next item, pos--    ->  (15)
        WaitForArrayItemBody_17 = 17,


        //Expecting:
        // r,u,e           - letters to put in buffer  ->  (18)
        // (space), \r,  
        //   \n, \t, \,    - check buffer. push back   ->  (B)
        True_18 = 18,


        //Expecting:
        // a,l,s,e         - letters to put in buffer  ->  (19)
        // (space), \r,  
        //   \n, \t, \,    - check buffer. push back   ->  (B)
        False_19 = 19,

        Finish_100 = 100
    }
}
