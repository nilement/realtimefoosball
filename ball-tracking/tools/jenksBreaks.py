from numpy import array
import jenkspy
# features  = [1060, 1291, 1081, 876, 1309, 882, 1294, 1380, 885, 220, 884, 535, 519, 868, 1072, 1082, 1294, 1068, 1380, 1096, 228, 1284, 863, 1299, 1086, 102, 1293, 878, 86, 1306, 381, 886, 1272, 1280, 1073, 1073, 1074, 697, 1081, 1085, 1055, 1067, 1297, 880, 1278, 1074, 369, 519, 84, 1081, 688, 229, 1380, 1090, 1318, 1079, 1094, 529, 1309, 370, 239, 1289, 226, 1284, 865, 1074, 225, 373, 873, 880, 1298, 379, 689, 377, 1281, 1083, 376, 1285, 528, 875, 1080, 1294, 1292, 1084, 223, 526, 1304, 1282, 230, 1082, 1301, 1062, 230, 371, 875, 1087, 864, 886, 1292, 84, 684, 892, 230, 522, 1068, 688, 693, 95, 526, 106, 1092, 528, 91, 871, 1296, 701, 1297, 889, 1090, 1094, 229, 1078, 1077, 883, 1286, 1314, 1305, 867, 216, 1076, 1097, 884, 883, 1286, 1300, 1071, 708, 1091, 1380, 703, 695, 98, 525, 226, 696, 373, 1087, 1301, 1063, 1083, 1301, 1299, 876, 102, 1293, 692, 366, 535, 92, 229, 892, 243, 700, 1060, 1284, 1307, 92, 1297, 1075, 367, 532, 534, 81, 224, 1082, 528, 96, 1068, 1283, 1274, 526, 1068, 893, 1380, 531, 690, 87, 1072, 884, 701, 882, 1081, 384, 82, 1075, 1085, 1295, 882, 100, 367, 1077, 1296, 108, 1054, 97, 1083, 1299, 688, 1302, 225, 538, 1091, 877, 377, 868, 1288, 698, 1086, 1286, 222, 874, 1289, 700, 223, 88, 1279, 1308, 524, 236, 89, 869, 881, 703, 1089, 227, 891, 93, 104, 366, 1380, 1078, 1309, 873, 1089, 700, 686, 706, 1088, 98, 872, 1290, 880, 1285, 1305, 528, 381, 1303, 1292, 1083, 96, 379, 1306, 1070, 1296, 523, 1287, 94, 96, 226, 215, 1294, 1095, 1315, 533, 528, 889,
# 1062, 694, 702, 886, 89, 1299, 235, 870, 1298, 867, 1087, 378, 1295, 532, 1087, 698, 1092, 1084, 1286, 1288, 364, 1082, 1058, 87, 219, 887, 1298, 693, 1306, 374, 891, 377, 1073, 94, 371, 1065, 699, 1275, 228, 533, 528, 873, 1284, 1069, 1083, 369, 233, 704, 1071, 1301, 1380, 1090, 80, 1304, 238, 1281, 705, 379, 895, 373, 535, 529, 3, 530, 1052, 1289, 98, 1065, 1301, 1087, 235, 236, 698, 365, 1268, 687, 1056, 93, 873, 538, 1079, 1094, 378, 91, 107, 879, 222, 689, 1059, 97, 1304, 699, 876, 230, 375, 885, 92, 885, 1300, 521, 239, 1286, 689, 1072, 98, 370, 1061, 1094, 882, 105, 370, 886, 1075, 890, 99, 225, 1080, 863, 90, 1285, 1315, 215, 1289, 884, 219, 706, 382, 1075, 1380, 872, 536, 234, 1084, 1304, 97, 888, 533, 532, 884, 225, 230, 377, 229, 1306, 235, 381, 1086, 76, 85, 233, 887, 520, 99, 364, 1287, 96, 224, 1289, 1064, 376, 1270, 104, 372, 1291, 1088, 1072, 1298, 869, 95, 91, 696, 1380, 1290, 1301, 1094, 228, 223, 1281, 1279, 1079, 1066, 223, 1076, 230, 533, 1096, 234, 526, 220, 866, 240, 695, 1087, 87, 525, 1301, 1064, 879, 691, 95, 684, 688, 704, 100, 1077, 882, 1303, 1299, 1310, 703, 1297, 219,
# 374, 695, 1295, 105, 99, 375, 1276, 701, 1064, 1303, 1085, 888, 362, 520, 705, 536, 1093, 89, 229, 1303, 238, 227, 380, 538, 870, 1293, 874, 532, 1080, 521, 881, 886, 1082, 1076, 866, 528, 685, 689, 1072, 237, 1074, 1048, 103, 870, 1304, 884, 223, 1084, 688, 362, 1270, 1080, 1077, 1071, 705, 95, 364, 1271, 1090, 1282, 382, 1066, 519, 1086, 872, 704, 891, 95, 519, 882, 1311, 1308, 1315, 531, 881, 221, 519, 871, 877, 1288, 1298, 224, 1299, 693, 692, 876, 90, 875, 884, 105, 880, 91, 687, 1319, 694, 691, 373, 535, 1071, 241, 1288, 686, 1079, 533, 518, 1292, 539, 686, 93, 84, 1089, 709, 368, 1079, 890, 1291, 1077, 94, 868, 1088, 697, 1307, 1068, 98, 1297, 99, 1073, 219, 1317, 1072, 1287, 97, 370, 1092, 1087, 1302, 1298, 379, 366, 363, 1321, 698, 382, 880, 1305, 887, 695, 1083, 217, 704, 1083, 238, 1287, 867, 1300, 1296, 866, 88, 1280, 216, 875, 1380, 1278, 239, 380, 1297, 228, 1079, 103, 1305, 1304, 888, 871, 1075, 1282, 1282, 1070, 239, 1080, 518, 702, 882, 688, 534, 884, 869, 692, 703, 522, 524, 867, 1098, 883, 232, 376, 379, 536, 1293, 1063, 1298, 1295, 226, 1308, 100, 1079, 687, 535, 368, 691, 231, 1274, 366, 895, 534, 887, 236, 686, 882, 1081, 94, 1060, 882, 526, 1071, 95, 96, 1291, 1273, 1300, 1293, 872, 108, 373, 219, 237, 1316, 91, 87, 542, 1093, 1090, 1086, 522, 1281, 85, 1380, 869, 704, 96, 876, 1295, 369, 86, 525, 1266, 375, 229, 861, 238, 4, 1073, 1300, 111, 234, 218, 81, 377, 377, 1279, 97, 712, 380, 528, 887, 1085, 1299, 1297, 886, 1083, 871, 703, 1074, 874]

def getBreaks(features, groupsCnt):
    return jenkspy.jenks_breaks(features, groupsCnt)

# breaks = jenkspy.jenks_breaks(features, 8)
# print(breaks)