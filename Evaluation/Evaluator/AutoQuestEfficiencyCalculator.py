from xml.dom import minidom
import datetime

xml_doc = None


def read_xml_doc(file_path):
    global xml_doc
    xml_doc = minidom.parse(file_path)


def get_events():
    return xml_doc.getElementsByTagName('event')


def get_target_id(target_name):
    targets = xml_doc.getElementsByTagName('target')
    for target in targets:
        params = target.getElementsByTagName('param')
        for p in params:
            if p.getAttribute('name') == 'name' and p.getAttribute('value') == target_name:
                return target.getAttribute('id')


def calculate_automated_completion_time(file_path):
    '''
    task completion time for automated mode is calculated between AutomatedButton click and ExecuteButton click
    :param file_path:
    :return:
    '''
    read_xml_doc(file_path)
    automated_button_id = get_target_id('AutomatedButton')
    execute_button_id = get_target_id('ExecuteButton')
    start_timestamp = None
    end_timestamp = None
    events = get_events()
    for event in events:
        if event.getAttribute('type') == 'PointerClick':
            params = event.getElementsByTagName('param')
            is_start_event_found = False
            is_end_event_found = False
            for p in params:
                if p.getAttribute('name') == 'targetId' and p.getAttribute('value') == automated_button_id:
                    is_start_event_found = True
            if is_start_event_found:
                for p in params:
                    if p.getAttribute('name') == 'timestamp':
                        start_timestamp = p.getAttribute('value')
            for p in params:
                if p.getAttribute('name') == 'targetId' and p.getAttribute('value') == execute_button_id:
                    is_end_event_found = True
            if is_end_event_found:
                for p in params:
                    if p.getAttribute('name') == 'timestamp':
                        end_timestamp = p.getAttribute('value')
            if is_start_event_found and is_end_event_found:
                break
    if start_timestamp is None:
        raise Exception('AutomatedButton not clicked')
    if end_timestamp is None:
        print('Warning: ExecuteButton not clicked')
        last_event = events[len(events) - 1]
        params = last_event.getElementsByTagName('param')
        for p in params:
            if p.getAttribute('name') == 'timestamp':
                end_timestamp = p.getAttribute('value')

    start_datetime = datetime.datetime(1, 1, 1) + datetime.timedelta(microseconds=int(start_timestamp) / 10)
    end_datetime = datetime.datetime(1, 1, 1) + datetime.timedelta(microseconds=int(end_timestamp) / 10)
    print((end_datetime - start_datetime).total_seconds())


def calculate_manual_completion_time(file_path):
    read_xml_doc(file_path)
    record_button_id = get_target_id('RecordButton')
    execute_button_id = get_target_id('ExecuteButton')
    start_timestamp = None
    end_timestamp = None
    events = get_events()
    for event in events:
        if event.getAttribute('type') == 'PointerClick':
            params = event.getElementsByTagName('param')
            is_start_event_found = False
            is_end_event_found = False
            for p in params:
                if p.getAttribute('name') == 'targetId' and p.getAttribute('value') == record_button_id:
                    is_start_event_found = True
            if is_start_event_found:
                for p in params:
                    if p.getAttribute('name') == 'timestamp':
                        start_timestamp = p.getAttribute('value')
            for p in params:
                if p.getAttribute('name') == 'targetId' and p.getAttribute('value') == execute_button_id:
                    is_end_event_found = True
            if is_end_event_found:
                for p in params:
                    if p.getAttribute('name') == 'timestamp':
                        end_timestamp = p.getAttribute('value')
            if is_start_event_found and is_end_event_found:
                break
    if start_timestamp is None:
        raise Exception('RecordButton not clicked')
    if end_timestamp is None:
        print('Warning: ExecuteButton not clicked')
        last_event = events[len(events) - 1]
        params = last_event.getElementsByTagName('param')
        for p in params:
            if p.getAttribute('name') == 'timestamp':
                end_timestamp = p.getAttribute('value')

    start_datetime = datetime.datetime(1, 1, 1) + datetime.timedelta(microseconds=int(start_timestamp) / 10)
    end_datetime = datetime.datetime(1, 1, 1) + datetime.timedelta(microseconds=int(end_timestamp) / 10)
    print((end_datetime - start_datetime).total_seconds())
