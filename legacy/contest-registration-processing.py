import pandas as pd
import argparse
import sys
import os

"""
Process Contest Registration Excel files.

Reads an Excel file generated from Google Forms returns. 
The file should have 2 tabs: 'Robots' and 'Attendees'.

The input file is processed into a single output spreadsheet with multiple tabs.
The output will have a tab for 
 - every challenge, listing the entries
 - all the registered attendees along with their email address and under_18 flag
 - a summary of all the challenge entries in a single list
 - a summary showing the number of entries in each challenge

run like:

python process_contest.py "Spring 2026 Registration (Responses).xlsx"

or, with a custom output name:
python process_contest.py "Spring 2026 Registration.xlsx" --output "April_2026_Checklist.xlsx"

"""
def process_event_registration(input_excel_path, output_filename):
    """
    Processes a single Excel file with 'Robots' and 'Attendees' tabs
    into a structured competition checklist.
    """
    try:
        # 1. Load the specific tabs
        df_robots = pd.read_excel(input_excel_path, sheet_name='Robots')
        df_attendees = pd.read_excel(input_excel_path, sheet_name='Attendees')
    except Exception as e:
        print(f"Error reading Excel file: {e}")
        sys.exit(1)

    # Normalize helpers
    def clean_email(email): return str(email).strip().lower()
    def clean_class(c): return 'Junior' if 'Junior' in str(c) else 'Senior'

    # 2. Flatten Robot/Challenge Entries
    entries = []
    for _, row in df_robots.iterrows():
        email = clean_email(row['Email address'])
        builder = str(row['Name']).strip()
        entry_class = clean_class(row['Class of entry'])
        for i in range(1, 6):
            r_name = str(row.get(f'Robot {i} Name ?', '')).strip()
            chals = str(row.get(f'Enter Robot {i} in these Challenges', ''))
            if chals.lower() != 'nan' and chals.strip() != "":
                robot = r_name if (r_name and r_name.lower() != 'nan') else f"Unnamed (Slot {i})"
                for c in [x.strip() for x in chals.split(',') if x.strip()]:
                    entries.append({
                        'Email': email, 
                        'Builder': builder, 
                        'Class': entry_class, 
                        'Robot': robot, 
                        'Challenge': c
                    })
    
    df_flat = pd.DataFrame(entries)

    # 3. Process Attendees
    attendee_rows = []
    slots = [('Name of the first attendee?', 'First attendee is 18 or more  years of age'),
             ('Name of the 2nd attendee?', 'Is the 2nd attendee under 18 years of age?'),
             ('Name of the 3rd attendee?', 'Is the 3rd attendee under 18 years of age?'),
             ('Name of the 4th attendee?', 'Is the 4th attendee under 18 years of age?'),
             ('Name of the 5th attendee?', 'Is the 5th attendee under 18 years of age?'),
             ('Name of the 6th attendee?', 'Is the 6th attendee under 18 years of age?')]
    
    for _, row in df_attendees.iterrows():
        email = clean_email(row['Email address'])
        for name_col, age_col in slots:
            if name_col in df_attendees.columns and pd.notna(row[name_col]):
                name = str(row[name_col]).strip()
                if name and name.lower() != 'nan':
                    age_val = str(row[age_col]).strip()
                    # Determine Under 18 status
                    is_u18 = 'Yes' if ('Yes' in age_val and '18 or more' not in age_val) or \
                                      ('under 18' in age_val and 'No' not in age_val) else 'No'
                    attendee_rows.append({'Email': email, 'Name': name, 'Under 18': is_u18})
    
    df_att_flat = pd.DataFrame(attendee_rows)

    # 4. Export to Final Excel with Multiple Tabs
    with pd.ExcelWriter(output_filename) as writer:
        # Summary Tab
        summary = df_flat.groupby(['Class', 'Challenge']).size().unstack(fill_value=0)
        summary['Total'] = summary.sum(axis=1)
        summary.to_excel(writer, sheet_name='Summary_Stats')

        # Master Lists
        df_att_flat.sort_values('Name').to_excel(writer, sheet_name='Master_Attendees', index=False)
        df_flat.sort_values(['Builder', 'Robot']).to_excel(writer, sheet_name='Master_Robots', index=False)

        # Challenge-Specific Tabs
        challenges = sorted(df_flat['Challenge'].unique())
        for chal in challenges:
            for cls in ['Junior', 'Senior']:
                subset = df_flat[(df_flat['Challenge'] == chal) & (df_flat['Class'] == cls)]
                if not subset.empty:
                    # Sort alphabetical and pick columns
                    subset = subset.sort_values('Builder')[['Builder', 'Robot']]
                    # Create tab name (e.g., J_Maze Solver) - Max 31 chars
                    prefix = 'J_' if cls == 'Junior' else 'S_'
                    clean_chal = chal.split(' ', 1)[1] if ' ' in chal else chal
                    sheet_name = (prefix + clean_chal)[:31].replace(':', '').replace('/', '-')
                    subset.to_excel(writer, sheet_name=sheet_name, index=False)

    print(f"Successfully generated: {output_filename}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Process Contest Registration Excel files.")
    parser.add_argument("input", help="Path to the input Excel file (must have 'Robots' and 'Attendees' tabs)")
    parser.add_argument("-o", "--output", help="Optional: Name of the output file", default="Competition_Checklist.xlsx")
    
    args = parser.parse_args()

    if not os.path.exists(args.input):
        print(f"Error: The file '{args.input}' does not exist.")
        sys.exit(1)

    process_event_registration(args.input, args.output)