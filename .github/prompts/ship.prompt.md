--- 
description: Generates a structured session summary of changes, modified components, and test results for PR creation. 
--- 

# Prompt: Custom /ship Session Summary 

Summarize all work completed in the current session. 
Format the output into three distinct sections: 
1. **Modified & Created Files:** List all modified components, models, and services. 
2. **Key Architectural Decisions:** Highlight skills applied, instructions updated, and state management logic. 
3. **Validation & Verification:** Confirm test execution status (e.g., Roslyn build status and Playwright MCP smoke test results).

