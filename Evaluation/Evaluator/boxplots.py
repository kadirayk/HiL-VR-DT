import numpy as np
import matplotlib.pyplot as plt

SUS_manual = [50, 67.5, 77.5, 35, 92.5, 60, 27.5, 72.5, 60, 40, 45, 75, 95, 95]
SUS_auto = [77.5, 82.5, 80, 97.5, 92.5, 82.5, 87.5, 85, 70, 47.5, 60, 70, 95, 100]
presence = [78, 82, 71, 86, 78, 76, 63, 73, 65, 39, 69, 49, 96, 92]
effectiveness_vr_manual = [100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 100, 100, 100]
effectiveness_vr_auto = [100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 100, 100, 100]
efficiency_manual = [346, 301, 114, 52, 131, 87, 146, 94, 272, 139, 70, 187, 298]
efficiency_auto = [106, 101, 59, 84, 79, 56, 75, 69, 351, 82, 167, 103, 116]
ages = [24, 31, 24, 26, 29, 26, 32, 27, 63, 64, 27, 47, 27, 23]
programming_exp = [5, 4, 5, 5, 4, 3, 4, 3, 3, 3, 1, 1, 2, 1]
vr_exp = [1, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 1, 1, 2]
index = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14]

def plot_satisfaction():
    data = [presence, SUS_manual, SUS_auto]

    fig1, ax1 = plt.subplots()
    #ax1.set_title('Basic Plot')
    ax1.boxplot(data)
    ax1.yaxis.grid(True, linestyle='-', which='major', color='lightgrey',
                   alpha=0.5)
    plt.xticks([1, 2, 3], ['Presence Percentage\nScore', 'SUS for\nManual Mode', 'SUS for\nAutomated Mode'])
    plt.savefig('box_plot_satisfaction.png')

def plot_efficiency():
    data = [efficiency_manual, efficiency_auto]
    fig1, ax1 = plt.subplots()
    ax1.boxplot(data)
    ax1.yaxis.grid(True)
    plt.xticks([1, 2], ['Task Completion Time\nin Seconds for\nManual Mode', 'Task Completion Time\nin Seconds for\nAutomated Mode'])
    plt.savefig('box_plot_efficiency.png')

plot_satisfaction()
plot_efficiency()