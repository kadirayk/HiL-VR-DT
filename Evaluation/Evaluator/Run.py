from AutoQuestEfficiencyCalculator import *

file_paths_for_automated = [
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\1_test\automated\1_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\2_test\automated\2_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\3_test\automated\3_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\4_test\automated\4_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\5_test\automated\5_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\6_test\automated\6_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\7_test\automated\7_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\8_test\automated\8_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\9_test\automated\9_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\10_test\automated\10_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\11_test\automated\11_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\12_test\automated\12_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\13_test\automated\13_test_automated.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\14_test\automated\14_test_automated.log'
]

file_paths_for_manual = [
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\1_test\manual\1_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\2_test\manual\2_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\3_test\manual\3_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\4_test\manual\4_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\5_test\manual\5_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\6_test\manual\6_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\7_test\manual\7_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\8_test\manual\8_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\9_test\manual\9_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\10_test\manual\10_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\11_test\manual\11_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\12_test\manual\12_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\13_test\manual\13_test_manual.log',
    r'C:\Users\Kadiray\Thesis\VR\Evaluation\AutoQuestLogs\14_test\manual\14_test_manual.log',
]

print("automated task completion time")
for file_path in file_paths_for_automated:
    calculate_automated_completion_time(file_path)

print("manual task completion time")
for file_path in file_paths_for_manual:
    calculate_manual_completion_time(file_path)

print("automated total time")
for file_path in file_paths_for_automated:
    calculate_total_duration(file_path)

print("manual total time")
for file_path in file_paths_for_manual:
    calculate_total_duration(file_path)
