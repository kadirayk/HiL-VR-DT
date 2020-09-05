import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import matplotlib
import numpy as np

SUS_manual = [50, 67.5, 77.5, 35, 92.5, 60, 27.5, 72.5, 60, 40, 45, 75, 95, 95]
SUS_auto = [77.5, 82.5, 80, 97.5, 92.5, 82.5, 87.5, 85, 70, 47.5, 60, 70, 95, 100]
presence = [78, 82, 71, 86, 78, 76, 63, 73, 65, 39, 69, 49, 96, 92]
effectiveness_vr_manual = [100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 100, 100, 100]
effectiveness_vr_auto = [100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 100, 100, 100]
efficiency_manual = [346, 301, 114, 52, 131, 87, 146, 94, 272, np.nan, 139, 70, 187, 298]
efficiency_auto = [106, 101, 59, 84, 79, 56, 75, 69, 351, np.nan, 82, 167, 103, 116]
ages = [24, 31, 24, 26, 29, 26, 32, 27, 63, 64, 27, 47, 27, 23]
programming_exp = [5, 4, 5, 5, 4, 3, 4, 3, 3, 3, 1, 1, 2, 1]
vr_exp = [1, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 1, 1, 2]
index = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14]

norm = matplotlib.colors.Normalize(vmin=1, vmax=14)
cmap = matplotlib.cm.get_cmap('tab20b')
p1 = mpatches.Patch(color=cmap(norm(1)), label='Participant 1')
p2 = mpatches.Patch(color=cmap(norm(2)), label='Participant 2')
p3 = mpatches.Patch(color=cmap(norm(3)), label='Participant 3')
p4 = mpatches.Patch(color=cmap(norm(4)), label='Participant 4')
p5 = mpatches.Patch(color=cmap(norm(5)), label='Participant 5')
p6 = mpatches.Patch(color=cmap(norm(6)), label='Participant 6')
p7 = mpatches.Patch(color=cmap(norm(7)), label='Participant 7')
p8 = mpatches.Patch(color=cmap(norm(8)), label='Participant 8')
p9 = mpatches.Patch(color=cmap(norm(9)), label='Participant 9')
p10 = mpatches.Patch(color=cmap(norm(10)), label='Participant 10')
p11 = mpatches.Patch(color=cmap(norm(11)), label='Participant 11')
p12 = mpatches.Patch(color=cmap(norm(12)), label='Participant 12')
p13 = mpatches.Patch(color=cmap(norm(13)), label='Participant 13')
p14 = mpatches.Patch(color=cmap(norm(14)), label='Participant 14')


def plot_SUS_manual():
    fig, axs = plt.subplots(nrows=1, ncols=3, figsize=(12, 4))

    fig.suptitle('SUS for Manual Mode')
    axs[0].scatter(ages, SUS_manual, c=index, cmap='tab20b', label=index)
    axs[1].scatter(programming_exp, SUS_manual, c=index, cmap='tab20b', label=index)
    axs[2].scatter(vr_exp, SUS_manual, c=index, cmap='tab20b', label=index)

    for ax in axs:
        ax.yaxis.grid(True)
        ax.xaxis.grid(True)
        ax.set_yticks([10, 20, 30, 40, 50, 60, 70, 80, 90, 100])

    axs[0].set_xticks([20, 25, 30, 35, 40, 45, 50, 55, 60, 65])
    axs[0].set_ylabel('SUS Score')
    axs[1].set_xticks([1, 2, 3, 4, 5])
    axs[2].set_xticks([1, 2, 3, 4, 5])
    axs[0].set_xlabel('Age')
    axs[1].set_xlabel('Programming Experience')
    axs[2].set_xlabel('VR Experience')
    lgd = plt.legend(handles=[p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14],
                     loc='upper center', bbox_to_anchor=(-0.8, -0.2), ncol=7)
    plt.savefig('x_sus_manual.png', bbox_extra_artists=(lgd,), bbox_inches='tight')


def plot_SUS_auto():
    fig, axs = plt.subplots(nrows=1, ncols=3, figsize=(12, 4))

    fig.suptitle('SUS for Automated Mode')
    axs[0].scatter(ages, SUS_auto, c=index, cmap='tab20b')
    axs[1].scatter(programming_exp, SUS_auto, c=index, cmap='tab20b')
    axs[2].scatter(vr_exp, SUS_auto, c=index, cmap='tab20b')

    for ax in axs:
        ax.yaxis.grid(True)
        ax.xaxis.grid(True)
        ax.set_yticks([10, 20, 30, 40, 50, 60, 70, 80, 90, 100])

    axs[0].set_xticks([20, 25, 30, 35, 40, 45, 50, 55, 60, 65])
    axs[0].set_ylabel('SUS Score')
    axs[1].set_xticks([1, 2, 3, 4, 5])
    axs[2].set_xticks([1, 2, 3, 4, 5])
    axs[0].set_xlabel('Age')
    axs[1].set_xlabel('Programming Experience')
    axs[2].set_xlabel('VR Experience')
    lgd = plt.legend(handles=[p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14], loc='upper center',
               bbox_to_anchor=(-0.8, -0.2), ncol=7)
    plt.savefig('x_sus_auto.png', bbox_extra_artists=(lgd,), bbox_inches='tight')


def plot_presence():
    fig, axs = plt.subplots(nrows=1, ncols=3, figsize=(12, 4))

    fig.suptitle('Presence Questionnaire')
    axs[0].scatter(ages, presence, c=index, cmap='tab20b')
    axs[1].scatter(programming_exp, presence, c=index, cmap='tab20b')
    axs[2].scatter(vr_exp, presence, c=index, cmap='tab20b')

    for ax in axs:
        ax.yaxis.grid(True)
        ax.xaxis.grid(True)
        ax.set_yticks([10, 20, 30, 40, 50, 60, 70, 80, 90, 100])

    axs[0].set_xticks([20, 25, 30, 35, 40, 45, 50, 55, 60, 65])
    axs[0].set_ylabel('Presence Percentage Score')
    axs[1].set_xticks([1, 2, 3, 4, 5])
    axs[2].set_xticks([1, 2, 3, 4, 5])
    axs[0].set_xlabel('Age')
    axs[1].set_xlabel('Programming Experience')
    axs[2].set_xlabel('VR Experience')
    lgd = plt.legend(handles=[p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14], loc='upper center',
               bbox_to_anchor=(-0.8, -0.2), ncol=7)
    plt.savefig('x_presence.png', bbox_extra_artists=(lgd,), bbox_inches='tight')


def plot_efficiency_auto():
    fig, axs = plt.subplots(nrows=1, ncols=3, figsize=(12, 4))

    fig.suptitle('Efficiency for Automated Mode')
    axs[0].scatter(ages, efficiency_auto, c=index, cmap='tab20b')
    axs[1].scatter(programming_exp, efficiency_auto, c=index, cmap='tab20b')
    axs[2].scatter(vr_exp, efficiency_auto, c=index, cmap='tab20b')

    for ax in axs:
        ax.yaxis.grid(True)
        ax.xaxis.grid(True)
        ax.set_yticks([0, 60, 120, 180, 240, 300, 360])

    axs[0].set_xticks([20, 25, 30, 35, 40, 45, 50, 55, 60, 65])
    axs[0].set_ylabel('Task Completion Time in Seconds')
    axs[1].set_xticks([1, 2, 3, 4, 5])
    axs[2].set_xticks([1, 2, 3, 4, 5])
    axs[0].set_xlabel('Age')
    axs[1].set_xlabel('Programming Experience')
    axs[2].set_xlabel('VR Experience')
    lgd = plt.legend(handles=[p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14], loc='upper center',
               bbox_to_anchor=(-0.8, -0.2), ncol=7)
    plt.savefig('x_efficiency_auto.png', bbox_extra_artists=(lgd,), bbox_inches='tight')


def plot_efficiency_manual():
    fig, axs = plt.subplots(nrows=1, ncols=3, figsize=(12, 4))

    fig.suptitle('Efficiency for Manual Mode')
    axs[0].scatter(ages, efficiency_manual, c=index, cmap='tab20b')
    axs[1].scatter(programming_exp, efficiency_manual, c=index, cmap='tab20b')
    axs[2].scatter(vr_exp, efficiency_manual, c=index, cmap='tab20b')

    for ax in axs:
        ax.yaxis.grid(True)
        ax.xaxis.grid(True)
        ax.set_yticks([0, 60, 120, 180, 240, 300, 360])

    axs[0].set_xticks([20, 25, 30, 35, 40, 45, 50, 55, 60, 65])
    axs[0].set_ylabel('Task Completion Time in Seconds')
    axs[1].set_xticks([1, 2, 3, 4, 5])
    axs[2].set_xticks([1, 2, 3, 4, 5])
    axs[0].set_xlabel('Age')
    axs[1].set_xlabel('Programming Experience')
    axs[2].set_xlabel('VR Experience')
    lgd = plt.legend(handles=[p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14], loc='upper center',
               bbox_to_anchor=(-0.8, -0.2), ncol=7)
    plt.savefig('x_efficiency_manual.png', bbox_extra_artists=(lgd,), bbox_inches='tight')


plot_SUS_manual()
plot_SUS_auto()
plot_presence()
plot_efficiency_auto()
plot_efficiency_manual()
