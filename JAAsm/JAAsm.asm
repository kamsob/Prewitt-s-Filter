.code
FilterAsm PROC
    ; rcx - input bitmap pointer, rdx - output bitmap pointer, r8 - width, r9 - height, stack - stride
    ; push registers
    push rbx
    push rbp
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15

    mov ebx, [rbp + 48]    
    mov rsi, rbx        ; rsi - stride
    sub rbx, r8         ; rbx - difference between stride and width

    inc rcx
    add rcx, rsi
    inc rdx
    add rdx, rsi

    
    mov r11, r8    ; r11 - i (column width-2)
    dec r11
    dec r11
    mov r12, r9    ; r12 - j (line height-2)
    dec r12
    dec r12


    cmp r12, 0
    jle ENDLOOP
CHECK_BEFORE_LOOP:
    cmp r11, 8
    jl LASTLOOP
    cmp r11, 0
    jle ENDLOOP

MAINLOOP:
; œrodka nie laduje bo i tak filtr = 0

; xmm2 left top     xmm3 middle top     xmm4 right top
; xmm5 left middle                      xmm6 right middle
; xmm7 left down    xmm8 middle down    xmm9 right down

; xmm0 and xmm1 results
    pxor xmm0, xmm0   ; 0 to horizontal filter result
    pxor xmm1, xmm1   ; 0 to vertical filter result

    mov r10, rcx

    dec r10
    movq xmm5, QWORD PTR [r10]
    pmovzxbw xmm5, xmm5
    
    sub r10, rsi
    movq xmm2, QWORD PTR [r10]
    pmovzxbw xmm2, xmm2
    
    inc r10
    movq xmm3, QWORD PTR [r10]
    pmovzxbw xmm3, xmm3

    inc r10
    movq xmm4, QWORD PTR [r10]
    pmovzxbw xmm4, xmm4
    
    add r10, rsi
    movq xmm6, QWORD PTR [r10]
    pmovzxbw xmm6, xmm6
    
    add r10, rsi
    movq xmm9, QWORD PTR [r10]
    pmovzxbw xmm9, xmm9

    dec r10
    movq xmm8, QWORD PTR [r10]
    pmovzxbw xmm8, xmm8

    dec r10
    movq xmm7, QWORD PTR [r10]
    pmovzxbw xmm7, xmm7
    
    ;horizontal filter
    psubw xmm0, xmm2
    psubw xmm0, xmm5
    psubw xmm0, xmm7

    paddw xmm0, xmm4
    paddw xmm0, xmm6
    paddw xmm0, xmm9

    ;vertizal filter
    psubw xmm1, xmm2
    psubw xmm1, xmm3
    psubw xmm1, xmm4

    paddw xmm1, xmm7
    paddw xmm1, xmm8
    paddw xmm1, xmm9

    vpabsw xmm1, xmm1       ; module of each word
    packuswb xmm1, xmm1     ; pack to bytes with saturation

    movq QWORD PTR [rdx], xmm1 ; save to result bitmap  

    add rcx, 8
    add rdx, 8
    sub r11, 8
    
    cmp r11, 0
    jle NEXT_ROW
    cmp r11, 8
    jl LASTLOOP
    
    
    jmp MAINLOOP
    
LASTLOOP:
    mov rax, 0
    mov r10, rcx
    sub r10, rsi
    dec r10
    
    mov r14, 0 ; result_vertical
    mov r15, 0 ; result_horizontal
    
    mov al, [r10]
    sub r15, rax
    sub r14, rax


    inc r10
    mov al, [r10]
    sub r14, rax


    inc r10
    mov al, [r10]
    sub r14, rax
    add r15, rax


    add r10, rsi
    mov al, [r10]
    add r15, rax


    sub r10, 2
    mov al, [r10]
    sub r15, rax

    
    add r10, rsi
    mov al, [r10]
    add r14, rax
    sub r15, rax

    
    inc r10
    mov al, [r10]
    add r14, rax


    inc r10
    mov al, [r10]
    add r14, rax
    add r15, rax

    mov rax, 0          ; Minimum value
    cmp r14, rax        ; compare r14 with 0
    jge IF_LARGER_THAN_ZERO    ; if r14 >= 0, jump to MORETHANZERO

    neg r14

IF_LARGER_THAN_ZERO:
    mov rax, 255        ; Maximum value
    cmp r14, rax
    cmovg r14, rax      ; If r15 > 255, set r14 to 255

    mov rax, r14
    mov [rdx], al
    
    inc rcx
    inc rdx 
    dec r11
    
    cmp r11, 0
    jle NEXT_ROW
    jmp LASTLOOP

    
NEXT_ROW:
    dec r12
    cmp r12, 0
    jle ENDLOOP

    mov r11, r8     ; reset column iterator
    dec r11
    dec r11         ; r11 = width - 2
    add rcx, rbx
    inc rcx
    inc rcx
    add rdx, rbx
    inc rdx
    inc rdx
    
    jmp CHECK_BEFORE_LOOP
    
ENDLOOP:
   
    pop r15
    pop r14
    pop r13
    pop r12
    pop rdi
    pop rsi
    pop rbp
    pop rbx

    ret
FilterAsm ENDP
END
